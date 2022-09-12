using GPManagementSytem.Database;
using GPManagementSytem.Models;
using GPManagementSytem.Services;
using GPManagementSytem.SessionManagement;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GPManagementSytem.Controllers
{
    public class BaseController : Controller
    {
        private DatabaseEntities _databaseEntities;
        public readonly ISessionManager SessionManager;
        public readonly IPracticeExternalService _practiceExternalService;

        public string getAttachmentPath = ConfigurationManager.AppSettings["attachmentPath"].ToString();

        public BaseController(ISessionManager sessionManager, IPracticeExternalService practiceExternalService)
        {
            databaseEntities = new DatabaseEntities();
            SessionManager = sessionManager;
            _practiceExternalService = practiceExternalService;
        }


        public DatabaseEntities databaseEntities
        {
            get { return _databaseEntities; }
            private set { _databaseEntities = value; }
        }

        public string GeneratePassword()
        {
            var myPwd = Guid.NewGuid().ToString().Substring(0, 8) + DateTime.Now.Second.ToString();

            return myPwd;
        }

        public void ShowChangesPendingCount()
        {
            int showCount = _practiceExternalService.GetAllPending().Count();

            ViewData["changesCount"] = showCount;
        }

        public string AcademicYearDD()
        {
            List<SelectListItem> myList = new List<SelectListItem>();

            int thisYear = DateTime.Now.Year;

            var getThisYear = DateTime.Now.Year.ToString();
            var getNextYear = (thisYear + 1).ToString();
            var getLastYear = (thisYear - 1).ToString();
            var getYearAfter = (thisYear + 2).ToString();

            string showThisYear = getLastYear + " - " + getThisYear;
            string showNextYear = getThisYear + " - " + getNextYear;


            //myList.Add(new SelectListItem { Value = getLastYear, Text = getLastYear });
            myList.Add(new SelectListItem { Value = showThisYear, Text = showThisYear, Selected = true });
            myList.Add(new SelectListItem { Value = showNextYear, Text = showNextYear });


            ViewData["AcademicYearDD"] = myList;

            return showThisYear;
        }

        public string BuildEmail(Users cu, List<Users> ulist, EmailTemplates getType)
        {
            var myMail = getType;
            int getEmailType = getType.EmailTypeId;

            var subject = myMail.Subject;
            string getURL = string.Format("{0}://{1}{2}", "https", Request.Url.Authority, Url.Content("~"));
            string GuidToIndentify = Guid.NewGuid().ToString();
            getURL = getURL + "/" + GuidToIndentify;

            var content = "";

            string getSendCode = Guid.NewGuid().ToString().Substring(0, 8) + DateTime.Now.Minute.ToString();

            switch (getEmailType)
            {
                case 1:

                    content = createTemplateEmailBody(getType.Body, cu.Firstname, getURL);
                    SendEmail(cu.Email, subject, content, getEmailType, cu.Id, getSendCode, GuidToIndentify, getType.AttachmentName);

                    break;

            }

            return getSendCode;

        }

        public void SendEmail(string emailAddress, string subject, string body, int typeId, int userId, string getSendCode, string GuidToIndentify, string myAttachment = null)
        {
            var mail = new MailMessage();

            string textBody = ConvertHtml(body);

            using (var SmtpServer = new SmtpClient(WebConfigurationManager.AppSettings["SmtpServer"]))
            {
                mail.From = new MailAddress(WebConfigurationManager.AppSettings["adminEmail"],
                                            WebConfigurationManager.AppSettings["adminName"]);

                mail.IsBodyHtml = true;
                mail.To.Add(emailAddress);
                mail.ReplyToList.Add(emailAddress);
                mail.Subject = subject;
                //mail.Body = body;

                mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(textBody, new ContentType("text/plain")));
                mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, new ContentType("text/html")));

                if (myAttachment != null)
                {
                    string uploadFolder = Server.MapPath(getAttachmentPath);

                    string getAttachment = uploadFolder + myAttachment;

                    Attachment attachment = new Attachment(getAttachment);
                    mail.Attachments.Add(attachment);
                    //logger.Info("File attached to email: " + myAttachment);
                }

                SmtpServer.Port = 587;
                //SmtpServer.Port = 25;
                SmtpServer.EnableSsl = true;
                SmtpServer.Credentials = new System.Net.NetworkCredential(WebConfigurationManager.AppSettings["SmtpUsername"],
                                                                          WebConfigurationManager.AppSettings["SmtpPassword"]);

                mail.Sender = new MailAddress(WebConfigurationManager.AppSettings["adminEmail"],
                                            WebConfigurationManager.AppSettings["adminName"]);

                try
                {
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmails"]))
                    {
                        SmtpServer.Send(mail);

                        //write email log 
                        DoEmailLog(getSendCode, typeId, userId);
                    }
                }
                catch (Exception e)
                {
                    //logger.Error("Send email error: " + getCurrentUser().Id);
                    //logger.Error(e);
                }

            }
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

        public string ConvertText(string path)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(path);

            StringWriter sw = new StringWriter();
            ConvertTo(doc.DocumentNode, sw);
            sw.Flush();
            return sw.ToString();
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

        private void ConvertContentTo(HtmlNode node, TextWriter outText)
        {
            foreach (HtmlNode subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outText);
            }
        }

        public void DoEmailLog(string sendCode, int typeId, int userId)
        {
            //var esl = new EmailSentLog();
            //esl.SendCode = sendCode;
            //esl.EmailTemplateId = typeId;
            //esl.DateSent = DateTime.Now;
            //esl.SentBy = getCurrentUser().Id;
            //esl.SentTo = userId;
            //dbAccess.AddEmailSentLog(esl);
        }

        public string createTemplateEmailBody(string bodytext, string firstname, string nochangeurl)
        {
            string body = string.Empty;

            using (StreamReader reader = new StreamReader(Server.MapPath("~/Templates/signup_email.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{firstname}", firstname);
            body = body.Replace("{bodytext}", bodytext);
            body = body.Replace("{nochangeurl}", nochangeurl);

            return body;

        }

        public PracticesExternal ParsePracticeToExternal(Practices practice)
        {
            PracticesExternal myPracticeExt = new PracticesExternal();

            myPracticeExt.PrimaryId = practice.Id;
            myPracticeExt.Surgery = practice.Surgery;
            myPracticeExt.SurgeryInUse = practice.SurgeryInUse;
            myPracticeExt.GP1 = practice.GP1;
            myPracticeExt.GP1Email = practice.GP1Email;
            myPracticeExt.Address1 = practice.Address1;
            myPracticeExt.Address2 = practice.Address2;
            myPracticeExt.Town = practice.Town;
            myPracticeExt.Postcode = practice.Postcode;
            myPracticeExt.Telephone = practice.Telephone;
            myPracticeExt.Fax = practice.Fax;
            myPracticeExt.PracticeManager = practice.PracticeManager;
            myPracticeExt.PMEmail = practice.PMEmail;
            myPracticeExt.GP2 = practice.GP2;
            myPracticeExt.GP2Email = practice.GP2Email;
            myPracticeExt.Website = practice.Website;
            myPracticeExt.GP3 = practice.GP3;
            myPracticeExt.GP3Email = practice.GP3Email;
            myPracticeExt.GP4 = practice.GP4;
            myPracticeExt.GP4Email = practice.GP4Email;
            myPracticeExt.AdditionalEmails = practice.AdditionalEmails;
            myPracticeExt.SupplierNumber = practice.SupplierNumber;
            myPracticeExt.ContactSurgery = practice.ContactSurgery;
            myPracticeExt.Notes = practice.Notes;
            myPracticeExt.AttachmentsAllocated = practice.AttachmentsAllocated;
            myPracticeExt.UCCTNotes = practice.UCCTNotes;
            myPracticeExt.QualityVisitDateR1 = practice.QualityVisitDateR1;
            myPracticeExt.QualityVisitNotes = practice.QualityVisitNotes;
            myPracticeExt.Active = practice.Active;
            myPracticeExt.Disabled = practice.Disabled;
            myPracticeExt.Queried = practice.Queried;
            myPracticeExt.ListSize = practice.ListSize;
            myPracticeExt.NewPractice = practice.NewPractice;
            myPracticeExt.AcademicYear = practice.AcademicYear;
            myPracticeExt.QualityVisitDate = practice.QualityVisitDate;
            myPracticeExt.OKToProceed = practice.OKToProceed;
            myPracticeExt.DataReviewDate = practice.DataReviewDate;
            myPracticeExt.TutorTrainingGPName = practice.TutorTrainingGPName;
            myPracticeExt.TutorTrainingDate = practice.TutorTrainingDate;
            myPracticeExt.DateCreated = practice.DateCreated;
            myPracticeExt.DateUpdated = practice.DateUpdated;
            myPracticeExt.UpdatedBy = practice.UpdatedBy;


            return myPracticeExt;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ShowChangesPendingCount();
        }
    }
}