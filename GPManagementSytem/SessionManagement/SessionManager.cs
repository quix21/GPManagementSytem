using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace GPManagementSytem.SessionManagement
{
    public class SessionManager : ISessionManager
    {
        private static string _userKey = "Username";

        private readonly HttpSessionState _sessionCollection;

        public SessionManager(HttpSessionState context)
        {
            _sessionCollection = context;
        }

        private HttpSessionState _session()
        {
            return _sessionCollection;
        }

        public void Clear()
        {
            _sessionCollection.Clear();
        }


        public void SetLoggedInUser(Users user)
        {
            _session()[_userKey] = user;
        }

    }
}