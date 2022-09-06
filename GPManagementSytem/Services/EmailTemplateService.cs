using GPManagementSytem.Database;
using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IDatabaseEntities _databaseEntities;

        public EmailTemplateService(IDatabaseEntities databaseEntities)
        {
            _databaseEntities = databaseEntities;
        }

        public IQueryable<EmailTemplates> AllNoTracking()
        {
            return _databaseEntities.EmailTemplates.AsNoTracking();
        }

        public List<EmailTemplates> GetAll()
        {
            return AllNoTracking().ToList();
        }

        public EmailTemplates GetById(int id)
        {
            return AllNoTracking().Where(x => x.Id == id).FirstOrDefault();
        }

        public EmailTemplates AddEmailTemplate(EmailTemplates emailTemplates)
        {
            return UpdateEmailTemplate(emailTemplates);
        }

        public EmailTemplates EditEmailTemplate(EmailTemplates emailTemplates)
        {
            return UpdateEmailTemplate(emailTemplates);
        }

        private EmailTemplates UpdateEmailTemplate(EmailTemplates emailTemplates)
        {
            var existingEntity = _databaseEntities.EmailTemplates.FirstOrDefault(x => x.Id == emailTemplates.Id);

            EmailTemplates entityToUpdate;

            if (existingEntity != null)
            {
                entityToUpdate = existingEntity;
            }
            else
            {
                entityToUpdate = emailTemplates;
            }

            entityToUpdate.Subject = emailTemplates.Subject;
            entityToUpdate.Body = emailTemplates.Body;
            entityToUpdate.AttachmentName = emailTemplates.AttachmentName;
            entityToUpdate.DateUpdated = emailTemplates.DateUpdated;
            entityToUpdate.UpdatedBy = emailTemplates.UpdatedBy;

            if (existingEntity == null)
            {
                _databaseEntities.EmailTemplates.Add(entityToUpdate);
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