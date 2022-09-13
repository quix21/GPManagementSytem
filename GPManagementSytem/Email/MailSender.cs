using GPManagementSytem.Services;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace GPManagementSytem.Email
{
    public class MailSender: IMailSender
    {
        private readonly IMailServerService _mailServerService;
        /// <summary>
        /// Initializes a new instance of the <see cref="MailSender"/> class.
        /// </summary>
        /// <param name="mailServerService">The mail server service.</param>
        public MailSender(IMailServerService mailServerService)
        {
            _mailServerService = mailServerService;
        }
        /// <summary>
        /// Send an email.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="from">From.</param>
        /// <param name="fromname"></param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="cc">The CC.</param>
        /// <param name="bcc">The BCC.</param>
        /// <returns>Boolean indicating the email was composed and sent.</returns>
        public bool SendMail(string to, string from, string fromname, string subject, string body, string[] cc = null,
            string[] bcc = null, string attachment = null)
        {
            return SendMailMessage(Mail(to, from, fromname, subject, body, cc, bcc, attachment));
        }
        /// <summary>
        /// Returns a <see cref="MailMessage"/> object.
        /// This does not send the message.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="from">From.</param>
        /// <param name="fromname"></param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="cc">The cc.</param>
        /// <param name="bcc">The BCC.</param>
        /// <returns>The message.</returns>
        public MailMessage Mail(string to, string from, string fromname, string subject, string body, string[] cc = null, string[] bcc = null, string attachment = null)
        {
            var mailMessage = new MailMessage();
            mailMessage.To.Add(to);
            mailMessage.From = new MailAddress(from, fromname);

            if (cc != null)
            {
                foreach (var ccAddress in cc)
                {
                    mailMessage.CC.Add(ccAddress);
                }
            }

            if (bcc != null)
            {
                foreach (var bccAddress in bcc)
                {
                    mailMessage.Bcc.Add(bccAddress);
                }
            }

            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.ReplyToList.Add(from);

            string textBody = ConvertHtml(body);

            mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(textBody, new ContentType("text/plain")));
            mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, new ContentType("text/html")));


            if (attachment != null)
            {
                
                Attachment myAttachment = new Attachment(attachment);
                mailMessage.Attachments.Add(myAttachment);
 
            }

            mailMessage.Sender = new MailAddress(from, fromname);

            return mailMessage;
        }
        /// <summary>
        /// Sends the mail message.
        /// </summary>
        /// <param name="mailMessage">The mail message.</param>
        /// <returns>Boolean indicating the email was composed and sent.</returns>
        public bool SendMailMessage(MailMessage mailMessage)
        {
            return _mailServerService.SendMail(mailMessage);
        }

        public string ConvertHtml(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            StringWriter sw = new StringWriter();
            ConvertTo(doc.DocumentNode, sw);
            sw.Flush();
            return sw.ToString();
        }

        public void ConvertTo(HtmlNode node, TextWriter outText)
        {
            string html;
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // don't output comments
                    break;

                case HtmlNodeType.Document:
                    ConvertContentTo(node, outText);
                    break;

                case HtmlNodeType.Text:
                    // script and style must not be output
                    string parentName = node.ParentNode.Name;
                    if ((parentName == "script") || (parentName == "style"))
                        break;

                    // get text
                    html = ((HtmlTextNode)node).Text;

                    // is it in fact a special closing node output as text?
                    if (HtmlNode.IsOverlappedClosingElement(html))
                        break;

                    // check the text is meaningful and not a bunch of whitespaces
                    if (html.Trim().Length > 0)
                    {
                        outText.Write(HtmlEntity.DeEntitize(html));
                    }
                    break;

                case HtmlNodeType.Element:
                    switch (node.Name)
                    {
                        case "p":
                            // treat paragraphs as crlf
                            outText.Write("\r\n");
                            break;
                    }

                    if (node.HasChildNodes)
                    {
                        ConvertContentTo(node, outText);
                    }
                    break;
            }
        }

        private void ConvertContentTo(HtmlNode node, TextWriter outText)
        {
            foreach (HtmlNode subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outText);
            }
        }
    }
}