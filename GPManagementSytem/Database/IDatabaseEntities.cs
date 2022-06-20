﻿using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Database
{
    public interface IDatabaseEntities
    {
        IDbSet<Users> Users { get; set; }

        int SaveChanges();
    }
}