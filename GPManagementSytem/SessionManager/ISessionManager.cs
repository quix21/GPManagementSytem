﻿using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPManagementSytem.SessionManager
{
    public interface ISessionManager
    {
        void Clear();
        void SetLoggedInUser(Users user);
    }
}