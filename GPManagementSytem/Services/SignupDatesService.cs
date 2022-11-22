using GPManagementSytem.Database;
using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Services
{
    public class SignupDatesService: ISignupDatesService
    {
        private readonly IDatabaseEntities _databaseEntities;

        public SignupDatesService(IDatabaseEntities databaseEntities)
        {
            _databaseEntities = databaseEntities;
        }

        public IQueryable<SignupDates> AllNoTracking()
        {
            return _databaseEntities.SignupDates.AsNoTracking();
        }

        public List<SignupDates> GetAll()
        {
            return AllNoTracking().ToList();
        }

        public SignupDates AddSignupDate(SignupDates signupDates)
        {
            return UpdateSignupDates(signupDates);
        }

        public SignupDates EditSignupDate(SignupDates signupDates)
        {
            return UpdateSignupDates(signupDates);
        }

        private SignupDates UpdateSignupDates(SignupDates signupDates)
        {
            var existingEntity = _databaseEntities.SignupDates.FirstOrDefault(x => x.Id == signupDates.Id);

            SignupDates entityToUpdate;

            if (existingEntity != null)
            {
                entityToUpdate = existingEntity;
            }
            else
            {
                entityToUpdate = signupDates;
            }

            entityToUpdate.Year2Wk1From = signupDates.Year2Wk1From;
            entityToUpdate.Year2Wk1To = signupDates.Year2Wk1To;
            entityToUpdate.Year2Wk2From = signupDates.Year2Wk2From;
            entityToUpdate.Year2Wk2To = signupDates.Year2Wk2To;
            entityToUpdate.Year2Wk3From = signupDates.Year2Wk3From;
            entityToUpdate.Year2Wk3To = signupDates.Year2Wk3To;
            entityToUpdate.Year2Wk4From = signupDates.Year2Wk4From;
            entityToUpdate.Year2Wk4To = signupDates.Year2Wk4To;
            entityToUpdate.Year2Wk5From = signupDates.Year2Wk5From;
            entityToUpdate.Year2Wk5To = signupDates.Year2Wk5To;
            entityToUpdate.Year2Wk6From = signupDates.Year2Wk6From;
            entityToUpdate.Year2Wk6To = signupDates.Year2Wk6To;
            entityToUpdate.Year3B1From = signupDates.Year3B1From;
            entityToUpdate.Year3B1To = signupDates.Year3B1To;
            entityToUpdate.Year3B2From = signupDates.Year3B2From;
            entityToUpdate.Year3B2To = signupDates.Year3B2To;
            entityToUpdate.Year3B3From = signupDates.Year3B3From;
            entityToUpdate.Year3B3To = signupDates.Year3B3To;
            entityToUpdate.Year3B4From = signupDates.Year3B4From;
            entityToUpdate.Year3B4To = signupDates.Year3B4To;
            entityToUpdate.Year3B5From = signupDates.Year3B5From;
            entityToUpdate.Year3B5To = signupDates.Year3B5To;
            entityToUpdate.Year3B6From = signupDates.Year3B6From;
            entityToUpdate.Year3B6To = signupDates.Year3B6To;
            entityToUpdate.Year3B7From = signupDates.Year3B7From;
            entityToUpdate.Year3B7To = signupDates.Year3B7To;
            entityToUpdate.Year4B1From = signupDates.Year4B1From;
            entityToUpdate.Year4B1To = signupDates.Year4B1To;
            entityToUpdate.Year4B2From = signupDates.Year4B2From;
            entityToUpdate.Year4B2To = signupDates.Year4B2To;
            entityToUpdate.Year4B3From = signupDates.Year4B3From;
            entityToUpdate.Year4B3To = signupDates.Year4B3To;
            entityToUpdate.Year4B4From = signupDates.Year4B4From;
            entityToUpdate.Year4B4To = signupDates.Year4B4To;
            entityToUpdate.Year4B5From = signupDates.Year4B5From;
            entityToUpdate.Year4B5To = signupDates.Year4B5To;
            entityToUpdate.Year4B6From = signupDates.Year4B6From;
            entityToUpdate.Year4B6To = signupDates.Year4B6To;
            entityToUpdate.Year4B7From = signupDates.Year4B7From;
            entityToUpdate.Year4B7To = signupDates.Year4B7To;
            entityToUpdate.Year4B8From = signupDates.Year4B8From;
            entityToUpdate.Year4B8To = signupDates.Year4B8To;
            entityToUpdate.Year5B1From = signupDates.Year5B1From;
            entityToUpdate.Year5B1To = signupDates.Year5B2To;
            entityToUpdate.Year5B2From = signupDates.Year5B2From;
            entityToUpdate.Year5B2To = signupDates.Year5B2To;
            entityToUpdate.Year5B3From = signupDates.Year5B3From;
            entityToUpdate.Year5B3To = signupDates.Year5B3To;
            entityToUpdate.Year5B4From = signupDates.Year5B4From;
            entityToUpdate.Year5B4To = signupDates.Year5B4To;
            entityToUpdate.Year5B5From = signupDates.Year5B5From;
            entityToUpdate.Year5B5To = signupDates.Year5B5To;
            entityToUpdate.Year5B6From = signupDates.Year5B6From;
            entityToUpdate.Year5B6To = signupDates.Year5B6To;
            entityToUpdate.AcademicYear = signupDates.AcademicYear;
            entityToUpdate.DateUpdated = signupDates.DateUpdated;
            entityToUpdate.UpdatedBy = signupDates.UpdatedBy;

            if (existingEntity == null)
            {
                _databaseEntities.SignupDates.Add(entityToUpdate);
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