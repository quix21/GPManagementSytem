using GPManagementSytem.Models;
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
        IDbSet<Practices> Practices { get; set; }
        IDbSet<PracticesExternal> PracticesExternal { get; set; }
        IDbSet<Allocations> Allocations { get; set; }
        IDbSet<Signupsendlog> SignupSendLog { get; set; }
        IDbSet<EmailTemplates> EmailTemplates { get; set; }
        IDbSet<QualityVisit> QualityVisit { get; set; }
        IDbSet<SignupDates> SignupDates { get; set; }

        int SaveChanges();
    }
}