using GPManagementSytem.Database;
using GPManagementSytem.SessionManager;
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

        public BaseController(ISessionManager sessionManager)
        {
            databaseEntities = new DatabaseEntities();
            SessionManager = sessionManager;
        }


        public DatabaseEntities databaseEntities
        {
            get { return _databaseEntities; }
            private set { _databaseEntities = value; }
        }
    }
}