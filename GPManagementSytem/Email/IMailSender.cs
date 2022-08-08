using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace GPManagementSytem.Email
{
    public interface IMailSender
    {
        /// <summary>
        /// Send an email.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="from">From.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="cc">The CC.</param>
        /// <param name="bcc">The BCC.</param>
        /// <returns>Boolean indicating the email was composed and sent.</returns>
        bool SendMail(string to, string from, string fromname, string subject, string body, string[] cc = null,
            string[] bcc = null);
        /// <summary>
        /// Returns a <see cref="MailMessage"/> object.
        /// This does not send the message.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="from">From.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="cc">The cc.</param>
        /// <param name="bcc">The BCC.</param>
        /// <returns>The message.</returns>
        MailMessage Mail(string to, string from, string fromname, string subject, string body, string[] cc = null, string[] bcc = null);
        /// <summary>
        /// Sends the mail message.
        /// </summary>
        /// <param name="mailMessage">The mail message.</param>
        /// <returns>Boolean indicating the email was composed and sent.</returns>
        bool SendMailMessage(MailMessage mailMessage);
    }
}