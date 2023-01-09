using GPManagementSytem.Database;
using GPManagementSytem.Email;
using GPManagementSytem.Models;
using GPManagementSytem.Services;
using GPManagementSytem.SessionManagement;
using HtmlAgilityPack;
using log4net;
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
        public readonly IMailSender _mailSender;
        public readonly IPracticeExternalService _practiceExternalService;
        public readonly ISignupSendLogService _signupSendLogService;

        public string getAttachmentPath = ConfigurationManager.AppSettings["attachmentPath"].ToString();

        public string adminEmail = ConfigurationManager.AppSettings["adminEmail"].ToString();
        public string adminName = ConfigurationManager.AppSettings["adminName"].ToString();


        public static ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BaseController(ISessionManager sessionManager, IMailSender mailSender, IPracticeExternalService practiceExternalService, ISignupSendLogService signupSendLogService)
        {
            databaseEntities = new DatabaseEntities();
            SessionManager = sessionManager;
            _mailSender = mailSender;
            _practiceExternalService = practiceExternalService;
            _signupSendLogService = signupSendLogService;
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

            var academicYear = AcademicYearDD();

            int getReturns = _signupSendLogService.GetAllNoActivity(academicYear).Where(x => x.DetailsUpdated == true).Count();

            ViewData["changesCount"] = showCount;
            ViewData["signupReturnsCount"] = getReturns;
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
            string getURL = string.Format("{0}://{1}{2}", "http", Request.Url.Authority, Url.Content("~"));
            string GuidToIndentify = "";

            var content = "";

            string getSendCode = Guid.NewGuid().ToString().Substring(0, 8) + DateTime.Now.Minute.ToString();

            switch (getEmailType)
            {
                case 1:

                    //if single user object exists then test email
                    if (cu != null)
                    {
                        content = createTemplateEmailBody(getType.Body, cu.Firstname, getURL, getURL);
                        SendEmail(cu.Email, subject, content, getEmailType, cu.Id, 0, getSendCode, GuidToIndentify, getType.AttachmentName);
                    }
                    else
                    {
                        foreach (var user in ulist)
                        {
                            GuidToIndentify = "";
                            GuidToIndentify = Guid.NewGuid().ToString();
                            var nochangeURL = getURL + "Login/ConfirmNoChanges?guid=" + GuidToIndentify;
                            var loginURL = getURL + "Login/PracticeLogin?guid=" + GuidToIndentify;

                            content = createTemplateEmailBody(getType.Body, user.Firstname, nochangeURL, loginURL);
                            SendEmail(user.Email, subject, content, getEmailType, user.Id, user.PracticeId, getSendCode, GuidToIndentify, getType.AttachmentName);
                        }
                    }
                                       
                    break;

            }

            return getSendCode;

        }

        public void SendEmail(string emailAddress, string subject, string body, int typeId, int userId, int PracticeId, string getSendCode, string GuidToIndentify, string myAttachment = null)
        {

            if (myAttachment != null)
            {
                string uploadFolder = Server.MapPath(getAttachmentPath);

                string getAttachment = uploadFolder + myAttachment;

            }

            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmails"]))
                {
                    _mailSender.SendMail(emailAddress, adminEmail, adminName, subject, body, null, null, GetAttachmentFilePath(myAttachment));

                    //write email log only if not a Preview Email
                    if (PracticeId !=0)
                    {
                        DoEmailLog(getSendCode, userId, PracticeId, GuidToIndentify);
                    }

                }
            }
            catch (Exception e)
            {
                logger.Error("Send email error: " + emailAddress);
                logger.Error(e);
            }

        }

        public string GetAttachmentFilePath(string myAttachment)
        {            

            string uploadFolder = Server.MapPath(getAttachmentPath);

            string getAttachment = uploadFolder + myAttachment;

            if (myAttachment != null)
            {
                return getAttachment;
            }
            else
            {
                return null;
            }

        }

 
        public void DoEmailLog(string sendCode, int userId, int practiceId, string Guid)
        {
            var esl = new Signupsendlog();
            esl.SendCode = sendCode;
            esl.AcademicYear = AcademicYearDD();
            esl.UserId = userId;
            esl.PracticeId = practiceId;
            esl.Guid = Guid;
            esl.NoChangesClicked = false;
            esl.DetailsUpdated = false;
            esl.DateSent = DateTime.Now;
            esl.DateActionTaken = null;
            esl.SentBy = Convert.ToInt32(Session["UserId"].ToString());

            _signupSendLogService.AddSignupSendLog(esl);
        }

        public string createTemplateEmailBody(string bodytext, string firstname, string nochangeurl, string loginurl)
        {
            string body = string.Empty;

            using (StreamReader reader = new StreamReader(Server.MapPath("~/Templates/signup_email.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{firstname}", firstname);
            body = body.Replace("{bodytext}", bodytext);
            body = body.Replace("{nochangeurl}", nochangeurl);
            body = body.Replace("{loginurl}", loginurl);

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