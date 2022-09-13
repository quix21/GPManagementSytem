using GPManagementSytem.Database;
using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Services
{
    public class SignupSendLogService: ISignupSendLogService
    {
        private readonly IDatabaseEntities _databaseEntities;

        public SignupSendLogService(IDatabaseEntities databaseEntities)
        {
            _databaseEntities = databaseEntities;
        }

        public IQueryable<Signupsendlog> AllNoTracking()
        {
            return _databaseEntities.SignupSendLog.AsNoTracking();
        }

        public List<Signupsendlog> GetAll()
        {
            return AllNoTracking().ToList();
        }

        public List<Signupsendlog> GetAllNoActivity(string academicYear)
        {
            return AllNoTracking().Where(x => x.NoChangesClicked == false && x.DetailsUpdated == false && x.AcademicYear == academicYear).ToList();
        }

        public List<Signupsendlog> GetAllByPratice(int practiceId)
        {
            return AllNoTracking().Where(x => x.PracticeId == practiceId).OrderByDescending(x => x.DateSent).ToList();
        }

        public List<Signupsendlog> GetBySendCode(string sendCode)
        {
            return AllNoTracking().Where(x => x.SendCode == sendCode).ToList();
        }

        public Signupsendlog GetByGuid(string guid)
        {
            return AllNoTracking().Where(x => x.Guid == guid).OrderByDescending(x => x.DateSent).FirstOrDefault();
        }

        public Signupsendlog AddSignupSendLog(Signupsendlog signupsendlog)
        {
            return UpdateSignupSendLog(signupsendlog);
        }

        public Signupsendlog EditSignupSendLog(Signupsendlog signupsendlog)
        {
            return UpdateSignupSendLog(signupsendlog);
        }

        private Signupsendlog UpdateSignupSendLog(Signupsendlog signupsendlog)
        {
            var existingEntity = _databaseEntities.SignupSendLog.FirstOrDefault(x => x.Id == signupsendlog.Id);

            Signupsendlog entityToUpdate;

            if (existingEntity != null)
            {
                entityToUpdate = existingEntity;
            }
            else
            {
                entityToUpdate = signupsendlog;
            }

            entityToUpdate.SendCode = signupsendlog.SendCode;
            entityToUpdate.AcademicYear = signupsendlog.AcademicYear;
            entityToUpdate.PracticeId = signupsendlog.PracticeId;
            entityToUpdate.Guid = signupsendlog.Guid;
            entityToUpdate.NoChangesClicked = signupsendlog.NoChangesClicked;
            entityToUpdate.DetailsUpdated = signupsendlog.DetailsUpdated;
            entityToUpdate.DateSent = signupsendlog.DateSent;
            entityToUpdate.DateActionTaken = signupsendlog.DateActionTaken;
            entityToUpdate.SentBy = entityToUpdate.SentBy;

            if (existingEntity == null)
            {
                _databaseEntities.SignupSendLog.Add(signupsendlog);
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