using GPManagementSytem.Services;
using GPManagementSytem.SessionManager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GPManagementSytem.Controllers
{
    public class LoginController : BaseController
    {
        private readonly IUserService _userService;

        private bool isImpersonate = Convert.ToBoolean(ConfigurationManager.AppSettings["isImpersonate"].ToString());

        //public static ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LoginController(IUserService userService, ISessionManager sessionManager) : base(sessionManager)
        {
            _userService = userService;

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


                //SessionManager.SetLoggedInUser(isUser);

                //logger.Info("Login successful for: " + isUser.Username);

                return this.RedirectToAction("ManageActionPlans", "Home");
            }
            else
            {
                ModelState.AddModelError("user", "Login details invalid");

                //logger.Info("Login failed for: " + username);
            }

            //TODO - audit logging

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
    }
}