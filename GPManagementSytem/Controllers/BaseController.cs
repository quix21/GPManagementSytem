using GPManagementSytem.Database;
using GPManagementSytem.Services;
using GPManagementSytem.SessionManagement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GPManagementSytem.Controllers
{
    public class BaseController : Controller
    {
        private DatabaseEntities _databaseEntities;
        public readonly ISessionManager SessionManager;
        public readonly IPracticeExternalService _practiceExternalService;

        public BaseController(ISessionManager sessionManager, IPracticeExternalService practiceExternalService)
        {
            databaseEntities = new DatabaseEntities();
            SessionManager = sessionManager;
            _practiceExternalService = practiceExternalService;
        }


        public DatabaseEntities databaseEntities
        {
            get { return _databaseEntities; }
            private set { _databaseEntities = value; }
        }

        public string GeneratePassword()
        {
            var myPwd = Guid.NewGuid().ToString().Substring(0, 8) + DateTime.Now.Second.ToString();

            return myPwd;
        }

        public void ShowChangesPendingCount()
        {
            int showCount = _practiceExternalService.GetAllPending().Count();

            ViewData["changesCount"] = showCount;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ShowChangesPendingCount();
        }
    }
}