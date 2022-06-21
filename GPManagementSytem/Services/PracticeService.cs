using GPManagementSytem.Database;
using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Services
{
    public class PracticeService: IPracticeService
    {
        private readonly IDatabaseEntities _databaseEntities;

        public PracticeService(IDatabaseEntities databaseEntities)
        {
            _databaseEntities = databaseEntities;
        }

        public IQueryable<Practices> AllNoTracking()
        {
            return _databaseEntities.Practices.AsNoTracking();
        }

        public List<Practices> GetAll()
        {
            return AllNoTracking().ToList();
        }
    }
}