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
            return AllNoTracking().OrderBy(x => x.Surgery).ToList();
        }

        public Practices GetById(int id)
        {
            return AllNoTracking().Where(x => x.Id == id).FirstOrDefault();
        }

        public Practices AddPractice(Practices practice)
        {
            return UpdatePractice(practice);
        }

        public Practices EditPractice(Practices practice)
        {
            return UpdatePractice(practice);
        }

        private Practices UpdatePractice(Practices practice)
        {
            var existingEntity = _databaseEntities.Practices.FirstOrDefault(x => x.Id == practice.Id);

            Practices entityToUpdate;

            if (existingEntity != null)
            {
                entityToUpdate = existingEntity;
            }
            else
            {
                entityToUpdate = practice;
            }

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