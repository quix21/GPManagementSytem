using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Services
{
    public interface IEmailTemplateService
    {
        List<EmailTemplates> GetAll();
        EmailTemplates GetById(int id);
        EmailTemplates AddEmailTemplate(EmailTemplates emailTemplates);
        EmailTemplates EditEmailTemplate(EmailTemplates emailTemplates);
    }
}