using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace GPManagementSytem.Services
{
    public class MailServerService: IMailServerService
    {
        public MailServerService()
        {

        }

        public bool SendMail(MailMessage mailMessage)
        {
            mailMessage.IsBodyHtml = true;

            using (var smtpServer = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"].ToString()))
            {
                smtpServer.Port = 587;
                smtpServer.EnableSsl = true;
                smtpServer.Credentials =
                    new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SmtpUsername"].ToString(),
                        ConfigurationManager.AppSettings["SmtpPassword"].ToString());
                try
                {
                    smtpServer.Send(mailMessage);
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return true;
        }
    }
}