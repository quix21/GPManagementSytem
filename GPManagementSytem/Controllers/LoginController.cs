using GPManagementSytem.Email;
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
        private readonly IMailSender _mailSender;

        private bool isImpersonate = Convert.ToBoolean(ConfigurationManager.AppSettings["isImpersonate"].ToString());

        private string adminEmail = ConfigurationManager.AppSettings["adminEmail"].ToString();
        private string adminName = ConfigurationManager.AppSettings["adminName"].ToString();


        public static ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LoginController(IUserService userService, ISessionManager sessionManager, IMailSender mailSender) : base(sessionManager)
        {
            _userService = userService;
            _mailSender = mailSender;

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

        public ActionResult PracticeLogin()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult PracticeLogin(FormCollection fc)
        {
            var username = fc["username"];
            var password = fc["password"];

            var isUser = _userService.LoginUserPractice(username, password, isImpersonate);

            if (isUser != null)
            {
                SignInRemember(username, true);
                Session["UserId"] = isUser.Id;


                SessionManager.SetLoggedInUser(isUser);

                logger.Info("Login successful for Practice user: " + isUser.Username);

                return this.RedirectToAction("EditPractice", "Home", new { id = isUser.PracticeId});
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