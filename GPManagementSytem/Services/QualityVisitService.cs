using GPManagementSytem.Database;
using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Services
{
    public class QualityVisitService: IQualityVisitService
    {
        private readonly IDatabaseEntities _databaseEntities;

        public QualityVisitService(IDatabaseEntities databaseEntities)
        {
            _databaseEntities = databaseEntities;
        }

        public IQueryable<QualityVisit> AllNoTracking()
        {
            return _databaseEntities.QualityVisit.AsNoTracking();
        }

        public List<QualityVisit> GetAll()
        {
            return AllNoTracking().ToList();
        }

        public List<QualityVisit> GetAllByPractice(int practiceId)
        {
            return AllNoTracking().Where(x => x.PracticeId == practiceId).OrderByDescending(x => x.DateOfVisit).ToList();
        }

        public List<QualityVisit> GetAllByVisitType(int visitTypeId)
        {
            return AllNoTracking().Where(x => x.VisitTypeId == visitTypeId).ToList();
        }

        public QualityVisit GetById(int id)
        {
            return AllNoTracking().Where(x => x.Id == id).FirstOrDefault();
        }

        public QualityVisit AddQualityVisit(QualityVisit qualityVisit)
        {
            return UpdateQualityVisit(qualityVisit);
        }

        public QualityVisit EditQualityVisit(QualityVisit qualityVisit)
        {
            return UpdateQualityVisit(qualityVisit);
        }

        private QualityVisit UpdateQualityVisit(QualityVisit qualityVisit)
        {
            var existingEntity = _databaseEntities.QualityVisit.FirstOrDefault(x => x.Id == qualityVisit.Id);

            QualityVisit entityToUpdate;

            if (existingEntity != null)
            {
                entityToUpdate = existingEntity;
            }
            else
            {
                entityToUpdate = qualityVisit;
            }

            entityToUpdate.PracticeId = qualityVisit.PracticeId;
            entityToUpdate.VisitTypeId = qualityVisit.VisitTypeId;
            entityToUpdate.VisitNumber = qualityVisit.VisitNumber;
            entityToUpdate.DateOfVisit = qualityVisit.DateOfVisit;
            entityToUpdate.DateCreated = qualityVisit.DateCreated;
            entityToUpdate.DateUpdated = qualityVisit.DateUpdated;
            entityToUpdate.UpdatedBy = qualityVisit.UpdatedBy;

            if (existingEntity == null)
            {
                _databaseEntities.QualityVisit.Add(qualityVisit);
            }

            Save();

            return entityToUpdate;
        }

        private bool Save()
        {
            return _databaseEntities.SaveChanges() > 0;
        }
    }
}