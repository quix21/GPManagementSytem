using GPManagementSytem.Email;
using GPManagementSytem.Helper;
using GPManagementSytem.Models;
using GPManagementSytem.Services;
using GPManagementSytem.SessionManagement;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GPManagementSytem.Controllers
{
    public class LoginController : BaseController
    {
        private readonly IUserService _userService;

        //private readonly IPracticeService _practiceService;

        private bool isImpersonate = Convert.ToBoolean(ConfigurationManager.AppSettings["isImpersonate"].ToString());

        //private string adminEmail = ConfigurationManager.AppSettings["adminEmail"].ToString();
        //private string adminName = ConfigurationManager.AppSettings["adminName"].ToString();



        public LoginController(IUserService userService, ISessionManager sessionManager,  IPracticeExternalService practiceExternalService, ISignupSendLogService signupSendLogService, IPracticeService practiceService, IMailSender mailSender) : base(sessionManager, mailSender, practiceExternalService, practiceService, signupSendLogService)
        {
            _userService = userService;

           // _practiceService = practiceService;

        }

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            var username = fc["username"];
            var password = fc["password"];

            var isUser = _userService.LoginUser(username, password, isImpersonate);

            if (isUser != null)
            {
                SignInRemember(username, true);
                Session["UserId"] = isUser.Id;
                Session["IsAdmin"] = true;


                SessionManager.SetLoggedInUser(isUser);

                logger.Info("Login successful for: " + isUser.Username);

                return this.RedirectToAction("ManagePractices", "Home");
            }
            else
            {
                ModelState.AddModelError("user", "Login details invalid");

                logger.Info("Login failed for: " + username);
            }

            //TODO - audit logging

            return View();
        }

        public ActionResult PracticeLogin(string guid = null)
        {
            if (string.IsNullOrEmpty(guid))
            {
                logger.Info("No GUID found in URL");
            }
            else
            {
                logger.Info("GUID found in URL: " + guid);
            }
                        

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult PracticeLogin(FormCollection fc)
        {
            var username = fc["username"];
            var password = fc["password"];

            var sentguid = fc["guid"];

            var isUser = _userService.LoginUserPractice(username, password, isImpersonate);

            if (string.IsNullOrEmpty(sentguid))
            {
                logger.Info("No GUID found for user: " + username);
            }
            else
            {
                logger.Info("GUID found for user: " + username + "(" + sentguid + ")");
            }

            if (isUser != null)
            {
                SignInRemember(username, true);
                Session["UserId"] = isUser.Id;
                Session["IsAdmin"] = false;

                SessionManager.SetLoggedInUser(isUser);

                logger.Info("Login successful for Practice user: " + isUser.Username);

                return this.RedirectToAction("AddAllocationExternal", "Home", new { id = isUser.PracticeId, guid = sentguid });
            }
            else
            {
                ModelState.AddModelError("user", "Login details invalid");

                logger.Info("Login failed for: " + username);
            }

            //TODO - audit logging

            return View();
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ResetPassword(FormCollection fc)
        {
            var username = fc["username"];

            var isUser = _userService.GetByUsernamePracticeOnly(username);


            if (isUser != null)
            {
                //TODO send email with newly generated password
                string newPassword = GeneratePassword();

                var emailBody = createEmailBody(isUser.Firstname, newPassword);

                

                _mailSender.SendMail(isUser.Username, adminEmail, adminName, "Password reset", emailBody, null, null);

                //update user record
                isUser.Pwd = newPassword;
                isUser.DateUpdated = DateTime.Now;
                isUser.UpdatedBy = 1; //hardcoded as sys admin

                _userService.EditUser(isUser);

                return this.RedirectToAction("PasswordReset", "Login");
            }
            else
            {
                ModelState.AddModelError("username", "Your details could not be found");

                return View();
            }

        }

        public ActionResult PasswordReset()
        {
            return View();
        }

        public ActionResult RegisterPractice()
        {
            var academicYear = AcademicYearDD();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateGoogleCaptcha]
        public ActionResult RegisterPractice(Practices practice)
        {
            if (ModelState.IsValid)
            {             

                PracticesExternal practicesExternal = ParsePracticeToExternal(practice);

                practicesExternal.Active = 1;
                practicesExternal.Queried = 0;
                practicesExternal.Disabled = 0;
                practicesExternal.NewPractice = true;
                practicesExternal.ContactSurgery = true;

                practicesExternal.RequestedBy = 0;
                practicesExternal.DateRequested = DateTime.Now;
                practicesExternal.ChangesApproved = false;

                _practiceExternalService.AddPractice(practicesExternal);

                return RedirectToAction("RegisterThanks");
            }
            else
            {
                return View(practice);
            }


        }

        public ActionResult RegisterThanks()
        {
            return View();
        }

        public ActionResult ConfirmNoChanges(string guid)
        {
            var myRecord = _signupSendLogService.GetByGuid(guid);

            bool isConfirmed = false;

            if (myRecord != null)
            {
                isConfirmed = true;

                myRecord.NoChangesClicked = true;
                myRecord.DateActionTaken = DateTime.Now;

                _signupSendLogService.EditSignupSendLog(myRecord);

                ViewBag.PracticeName = _practiceService.GetById(myRecord.PracticeId).Surgery;
            }


            ViewData["IsConfirmed"] = isConfirmed;

            return View();
        }

        private void SignInRemember(string userName, bool isPersistent = false)
        {
            // Clear any lingering authencation data
            FormsAuthentication.SignOut();

            // Write the authentication cookie
            FormsAuthentication.SetAuthCookie(userName, isPersistent);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            //Session.Abandon();
            Session.Clear();

            return View();
        }

        public ActionResult ForceLogout()
        {
            _Logout();
            return RedirectToAction("Index", "Login");
        }

        private ActionResult _Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            return RedirectToAction("Index", "Login");
        }

        public string createEmailBody(string firstname, string pwd)
        {
            string body = string.Empty;

            using (StreamReader reader = new StreamReader(Server.MapPath("~/Templates/resetpassword_email.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{firstname}", firstname);
            body = body.Replace("{pwd}", pwd);

            return body;

        }


    }
}