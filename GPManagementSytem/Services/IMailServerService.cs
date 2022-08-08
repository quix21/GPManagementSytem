using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace GPManagementSytem.Services
{
    public interface IMailServerService
    {
        bool SendMail(MailMessage mailMessage);
    }
}