using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Database
{
    [DbConfigurationType(typeof(MySql.Data.EntityFramework.MySqlEFConfiguration))]
    public class DatabaseEntities: DbContext, IDatabaseEntities
    {
        public IDbSet<Users> Users { get; set; }
        public IDbSet<Practices> Practices { get; set; }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }


}