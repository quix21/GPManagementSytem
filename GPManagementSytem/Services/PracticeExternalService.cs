using GPManagementSytem.Database;
using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Services
{
    public class PracticeExternalService: IPracticeExternalService
    {
        private readonly IDatabaseEntities _databaseEntities;

        public PracticeExternalService(IDatabaseEntities databaseEntities)
        {
            _databaseEntities = databaseEntities;
        }

        public IQueryable<PracticesExternal> AllNoTracking()
        {
            return _databaseEntities.PracticesExternal.AsNoTracking();
        }

        public List<PracticesExternal> GetAllApproved()
        {
            return AllNoTracking().Where(x => x.ChangesApproved == true).OrderByDescending(x => x.DateApproved).ToList();
        }

        public List<PracticesExternal> GetAllPending()
        {
            return AllNoTracking().Where(x => x.ChangesApproved == false).OrderByDescending(x => x.DateRequested).ToList();
        }

        public PracticesExternal GetById(int id)
        {
            return AllNoTracking().Where(x => x.Id == id).FirstOrDefault();
        }

        public PracticesExternal AddPractice(PracticesExternal practice)
        {
            return UpdatePractice(practice);
        }

        public PracticesExternal EditPractice(PracticesExternal practice)
        {
            return UpdatePractice(practice);
        }

        private PracticesExternal UpdatePractice(PracticesExternal practice)
        {
            var existingEntity = _databaseEntities.PracticesExternal.FirstOrDefault(x => x.Id == practice.Id);

            PracticesExternal entityToUpdate;

            if (existingEntity != null)
            {
                entityToUpdate = existingEntity;
            }
            else
            {
                entityToUpdate = practice;
            }

            entityToUpdate.PrimaryId = practice.PrimaryId;
            entityToUpdate.Surgery = practice.Surgery;
            entityToUpdate.SurgeryInUse = practice.SurgeryInUse;
            entityToUpdate.GP1 = practice.GP1;
            entityToUpdate.GP1Email = practice.GP1Email;
            entityToUpdate.Address1 = practice.Address1;
            entityToUpdate.Address2 = practice.Address2;
            entityToUpdate.Postcode = practice.Postcode;
            entityToUpdate.Telephone = practice.Telephone;
            entityToUpdate.Fax = practice.Fax;
            entityToUpdate.PracticeManager = practice.PracticeManager;
            entityToUpdate.PMEmail = practice.PMEmail;
            entityToUpdate.GP2 = practice.GP2;
            entityToUpdate.GP2Email = practice.GP2Email;
            entityToUpdate.Website = practice.Website;
            entityToUpdate.GP3 = practice.GP3;
            entityToUpdate.GP3Email = practice.GP3Email;
            entityToUpdate.GP4 = practice.GP4;
            entityToUpdate.GP4Email = practice.GP4Email;
            entityToUpdate.AdditionalEmails = practice.AdditionalEmails;
            entityToUpdate.SupplierNumber = practice.SupplierNumber;
            entityToUpdate.ContactSurgery = practice.ContactSurgery;
            entityToUpdate.Notes = practice.Notes;
            entityToUpdate.AttachmentsAllocated = practice.AttachmentsAllocated;
            entityToUpdate.UCCTNotes = practice.UCCTNotes;
            entityToUpdate.QualityVisitDateR1 = practice.QualityVisitDateR1;
            entityToUpdate.QualityVisitNotes = practice.QualityVisitNotes;
            entityToUpdate.Active = practice.Active;
            entityToUpdate.Disabled = practice.Disabled;
            entityToUpdate.Queried = practice.Queried;
            entityToUpdate.ListSize = practice.ListSize;
            entityToUpdate.NewPractice = practice.NewPractice;
            entityToUpdate.AcademicYear = practice.AcademicYear;
            entityToUpdate.QualityVisitDate = practice.QualityVisitDate;
            entityToUpdate.OKToProceed = practice.OKToProceed;
            entityToUpdate.DataReviewDate = practice.DataReviewDate;
            entityToUpdate.TutorTrainingGPName = practice.TutorTrainingGPName;
            entityToUpdate.TutorTrainingDate = practice.TutorTrainingDate;
            entityToUpdate.DateCreated = practice.DateCreated;
            entityToUpdate.DateUpdated = practice.DateUpdated;
            entityToUpdate.UpdatedBy = practice.UpdatedBy;
            entityToUpdate.DateRequested = practice.DateRequested;
            entityToUpdate.RequestedBy = practice.RequestedBy;
            entityToUpdate.DateApproved = practice.DateApproved;
            entityToUpdate.ApprovedBy = practice.ApprovedBy;
            entityToUpdate.ChangesApproved = practice.ChangesApproved;

            if (existingEntity == null)
            {
                _databaseEntities.Practices.Add(entityToUpdate);
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