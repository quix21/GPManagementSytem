using GPManagementSytem.Email;
using GPManagementSytem.Helper;
using GPManagementSytem.Models;
using GPManagementSytem.Security;
using GPManagementSytem.Services;
using GPManagementSytem.SessionManagement;
using GPManagementSytem.ViewModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace GPManagementSytem.Controllers
{
    //[CheckAuthorisation]
    public class HomeController : BaseController
    {
        //private readonly IPracticeService _practiceService;
        //private readonly IAllocationService _allocationService;
        private readonly IUserService _userService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ISignupDatesService _signupDatesService;

        public HomeController(IPracticeService practiceService, IPracticeExternalService practiceExternalService, IAllocationService allocationService, IUserService userService, IEmailTemplateService emailTemplateService, ISignupSendLogService signupSendLogService , ISessionManager sessionManager, IMailSender mailSender, ISignupDatesService signupDatesService) : base(sessionManager, mailSender, practiceExternalService, practiceService, allocationService, signupSendLogService)
        {
           // _practiceService = practiceService;
            //_allocationService = allocationService;
            _userService = userService;
            _emailTemplateService = emailTemplateService;
            _signupDatesService = signupDatesService;

        }

        public ActionResult AddAllocationExternal(int id, string guid = null)
        {
            var academicYear = AcademicYearDD();

            AllocationExternalViewModel myModel = new AllocationExternalViewModel();

            ////////////////

            //check if there are previous allocation requests pending
            //PracticesExternal changesPending = new PracticesExternal();

            //changesPending = _practiceExternalService.GetAllPending().Where(x => x.PrimaryId == id).FirstOrDefault();

            Allocations allocationRequestExists = new Allocations();

            allocationRequestExists = _allocationService.GetByPracticeAndYear(id, academicYear);

            if (allocationRequestExists != null)
            {
                return RedirectToAction("AllocationPending");
            }
            else
            {
                var myPractice = _practiceService.GetById(id);

                myPractice.PracticeStatusGroup = ManagePracticeStatusGroupGET(myPractice);
                ViewBag.Guid = guid;

                myModel.practice = myPractice;
            }

            var getDates = _signupDatesService.GetByAcademicYear(academicYear);

            int practiceId = id;

            Allocations myAllocation = new Allocations();

            myAllocation.PracticeId = practiceId;

            myModel.signupDates = getDates;
            myModel.allocations = myAllocation;

            myModel.allocations.AcademicYear = academicYear;

            ViewBag.Guid = guid;

            return View(myModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAllocationExternal(AllocationExternalViewModel allocationExternalViewModel, FormCollection fc)
        {
            string guid = fc["guid"].ToString();          

            Type type = typeof(Allocations);
            PropertyInfo[] properties = type.GetProperties();

            Type checkedType = typeof(AllocationExternalViewModel);
            PropertyInfo[] checkedProperties = checkedType.GetProperties();

            //iterate through the form collection. If the form field matches the name of the class property (which are indentically named) it means that the box has been ticked for that block/week. Thefore add 2 or 4 students to that block/week.

            //this persists from years 2, 3 and 4. In year 5, that value changes to 1 or 2 per block

            bool NoBlocksChecked = true;

            foreach (var key in fc.AllKeys)
            {
                var fieldName = key;

                foreach (PropertyInfo info in properties)
                {
                    var classPropertyName = info.Name;

                    if (fieldName == classPropertyName)
                    {
                        //check if year 2-4 or year 5
                        //if (fieldName.IndexOf("Year5") == -1)
                        //{
                            info.SetValue(allocationExternalViewModel.allocations, allocationExternalViewModel.GlobalNumberStudentsRequested, null);
                            NoBlocksChecked = false;

                            string dateChecked = fieldName + "Checked";

                            foreach (PropertyInfo checkedInfo in checkedProperties)
                            {
                                if (dateChecked == checkedInfo.Name)
                                {
                                    checkedInfo.SetValue(allocationExternalViewModel, true, null);
                                }
                            }
                    }
                }
            }


            if (NoBlocksChecked)
            {
                ModelState.AddModelError("NoBlocksChecked", "Please check at least one box for one year");
            }

            if (ModelState.IsValid)
            {
                //update contact details
                //check if any practice details have changed. If not then bypass the PracticeExternal/approval functionality

                if (ContactDetailsChanged(allocationExternalViewModel))
                {
                    ManagePracticeStatusGroupPOST(allocationExternalViewModel.practice);

                    PracticesExternal practicesExternal = ParsePracticeToExternal(allocationExternalViewModel.practice);

                    practicesExternal.RequestedBy = Convert.ToInt32(Session["UserId"].ToString());
                    practicesExternal.DateRequested = DateTime.Now;
                    practicesExternal.ChangesApproved = false;

                    _practiceExternalService.AddPractice(practicesExternal);
                }

                //add allocation details

                allocationExternalViewModel.allocations.ServiceContractReceived = 1;
                allocationExternalViewModel.allocations.AllocationApproved = false;

                allocationExternalViewModel.allocations.DateCreated = DateTime.Now;
                allocationExternalViewModel.allocations.UpdatedBy = allocationExternalViewModel.allocations.PracticeId;

                _allocationService.AddAllocation(allocationExternalViewModel.allocations);

                //set PracticeStatus to Active 
                var practice = _practiceService.GetById(allocationExternalViewModel.allocations.PracticeId);
                practice.PracticeStatusGroup = 1;
                ManagePracticeStatusGroupPOST(practice);

                practice.DateUpdated = DateTime.Now;
                practice.UpdatedBy = Convert.ToInt32(Session["UserId"].ToString());
                _practiceService.EditPractice(practice);

                //update sentlog to show that email has been responded to/allocation requested
                if (!String.IsNullOrEmpty(guid))
                {
                    var myRecord = _signupSendLogService.GetByGuid(guid);

                    if (myRecord != null)
                    {
                        myRecord.DetailsUpdated = true;
                        myRecord.DateActionTaken = DateTime.Now;

                        _signupSendLogService.EditSignupSendLog(myRecord);
                    }
                }

                return RedirectToAction("AllocationRequestSubmitted");
            }
            else
            {
                var getDates = _signupDatesService.GetByAcademicYear(allocationExternalViewModel.allocations.AcademicYear);

                allocationExternalViewModel.signupDates = getDates;

                ViewBag.Guid = guid;

                return View(allocationExternalViewModel);
            }

        }

        public bool ContactDetailsChanged(AllocationExternalViewModel allocationExternalViewModel)
        {
            bool detailsChanged = false;

            Practices currentPractice = _practiceService.GetById(allocationExternalViewModel.practice.Id);

            Type practiceType = typeof(Practices);
            PropertyInfo[] practiceProperties = practiceType.GetProperties();


            foreach (PropertyInfo info in practiceProperties)
            {
                var classPropertyName = info.Name;
                var showCurrentValue = info.GetValue(currentPractice);
                var showChangedValue = info.GetValue(allocationExternalViewModel.practice);

                int checkChange = Comparer.DefaultInvariant.Compare(showCurrentValue, showChangedValue);

                //ignore for automatic status change from dormant to active
                if (classPropertyName != "Active" && classPropertyName != "Queried")
                {
                    if (checkChange != 0)
                    {
                        detailsChanged = true;
                    }
                }
            }

            return detailsChanged;
        }

        public ActionResult AllocationRequestSubmitted()
        {
            return View();
        }

        public ActionResult AddSignupDate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSignupDate(SignupDates signupDates)
        {
            var academicYear = AcademicYearDD();

            signupDates.AcademicYear = academicYear;
            signupDates.DateUpdated = DateTime.Now;

            _signupDatesService.AddSignupDate(signupDates);

            return View();
        }

        public ActionResult EditSignupDate(int id)
        {
            var mySignupDate = _signupDatesService.GetAll().Where(x => x.Id == id).FirstOrDefault();

            return View(mySignupDate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSignupDate(SignupDates signupDates)
        {
            signupDates.DateUpdated = DateTime.Now;
            signupDates.UpdatedBy = Convert.ToInt32(Session["UserId"].ToString());

            _signupDatesService.EditSignupDate(signupDates);

            return View(signupDates);
        }

        public ActionResult AddPractice()
        {
            var academicYear = AcademicYearDD();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPractice(Practices practice)
        {
            if (ModelState.IsValid)
            {
                ManagePracticeStatusGroupPOST(practice);

                practice.DateCreated = DateTime.Now;
                practice.UpdatedBy = Convert.ToInt32(Session["UserId"].ToString());
                var newPractice = _practiceService.AddPractice(practice);

                if (CreatePMUser(newPractice) == false)
                {
                    logger.Error("Practice manager user not created (email exists): " + newPractice.PMEmail);
                }

                if (CreateGPUser(newPractice) == false)
                {
                    logger.Error("GP1 user not created (email exists): " + newPractice.GP1Email);
                }
                

                return RedirectToAction("ManagePractices");
            }
            else
            {
                return View();
            }
        }

        public ActionResult EditPracticeExternal(int id, string guid = null)
        {
            var academicYear = AcademicYearDD();

            //check if there are previous change requests pending
            PracticesExternal changesPending = new PracticesExternal();

            changesPending = _practiceExternalService.GetAllPending().Where(x => x.PrimaryId == id).FirstOrDefault();

            if (changesPending != null)
            {
                return RedirectToAction("ApprovalPending");
            }
            else
            {
                var myPractice = _practiceService.GetById(id);

                myPractice.PracticeStatusGroup = ManagePracticeStatusGroupGET(myPractice);
                ViewBag.Guid = guid;

                return View(myPractice);
            }

        }

        public ActionResult AllocationPending()
        {
            Session["UserId"] = 1;
            GenerateUsersFromExistingPractices();

            return View();
        }

        public ActionResult ApprovePracticeRegistration(int id)
        {
            var academicYear = AcademicYearDD();

            var regPractice = _practiceExternalService.GetById(id);

            Practices blankPractice = new Practices();

            regPractice.PracticeStatusGroup = ManagePracticeStatusGroupGET(blankPractice, regPractice);

            return View(regPractice);
        }

        [HttpPost]
        public ActionResult ApprovePracticeRegistration(PracticesExternal myPractice)
        {
            if (ModelState.IsValid)
            {
                ManagePracticeStatusGroupPOST(myPractice);

                var thisPractice = ParsePracticesExternalForRegistration(myPractice);

                //update original practice record
                thisPractice.DateCreated = DateTime.Now;
                thisPractice.UpdatedBy = Convert.ToInt32(Session["UserId"].ToString());

                var newPractice = _practiceService.AddPractice(thisPractice);

                if (CreatePMUser(newPractice) == false)
                {
                    logger.Error("Practice manager user not created (email exists): " + newPractice.PMEmail);
                }

                if (CreateGPUser(newPractice) == false)
                {
                    logger.Error("GP1 user not created (email exists): " + newPractice.GP1Email);
                }

                //update practiceexternal record and mark as approved
                myPractice.PrimaryId = newPractice.Id;
                myPractice.DateApproved = DateTime.Now;
                myPractice.ApprovedBy = Convert.ToInt32(Session["UserId"].ToString());
                myPractice.ChangesApproved = true;

                _practiceExternalService.EditPractice(myPractice);


                return RedirectToAction("ManagePracticesExternal");
            }
            else
            {
                var academicYear = AcademicYearDD();

                return View(myPractice);
            }
        }

        public bool CreatePMUser(Practices practice)
        {
            //create user 
            var getUsers = _userService.GetAll().Where(x => x.UserType > 1).ToList();

            bool isNew = true;

            var thisUser = getUsers.Where(x => x.Email == practice.PMEmail).ToList();

            if (thisUser.Count() > 0)
            {
                isNew = false;
            }

            if (isNew)
            {
                Users newUser = new Users();

                string firstName = "Firstname";
                string surname = "Surname";

                string[] getNames = practice.PracticeManager.Split(' ').ToArray();

                if (getNames.Length >= 2)
                {
                    firstName = getNames[0].ToString();
                    surname = getNames[1].ToString();
                }

                newUser.Firstname = firstName;
                newUser.Surname = surname;
                newUser.Email = practice.PMEmail;
                newUser.Username = practice.PMEmail;
                newUser.Pwd = GeneratePassword();
                newUser.UserType = 2;
                newUser.PracticeId = practice.Id;
                newUser.Year2 = false;
                newUser.Year3 = false;
                newUser.Year4 = false;
                newUser.Year5 = false;
                newUser.IsActive = true;
                newUser.DateCreated = DateTime.Now;
                newUser.DateUpdated = DateTime.Now;
                newUser.UpdatedBy = Convert.ToInt32(Session["UserId"].ToString());

                _userService.AddUser(newUser);

            }

            return isNew;
        }

        public bool CreatePMUser(PracticesExternal practice)
        {
            //create user 
            var getUsers = _userService.GetAll().Where(x => x.UserType > 1).ToList();

            bool isNew = true;

            var thisUser = getUsers.Where(x => x.Email == practice.PMEmail).ToList();

            if (thisUser.Count() > 0)
            {
                isNew = false;
            }

            if (isNew)
            {
                Users newUser = new Users();

                string firstName = "";
                string surname = "";

                string[] getNames = practice.PracticeManager.Split(' ').ToArray();

                if (getNames.Length >= 2)
                {
                    firstName = getNames[0].ToString();
                    surname = getNames[1].ToString();
                }


                newUser.Firstname = firstName;
                newUser.Surname = surname;
                newUser.Email = practice.PMEmail;
                newUser.Username = practice.PMEmail;
                newUser.Pwd = GeneratePassword();
                newUser.UserType = 2;
                newUser.PracticeId = practice.Id;
                newUser.Year2 = false;
                newUser.Year3 = false;
                newUser.Year4 = false;
                newUser.Year5 = false;
                newUser.IsActive = true;
                newUser.DateCreated = DateTime.Now;
                newUser.DateUpdated = DateTime.Now;
                newUser.UpdatedBy = Convert.ToInt32(Session["UserId"].ToString());

                _userService.AddUser(newUser);

            }

            return isNew;
        }

        public bool CreateGPUser(Practices practice)
        {
            //create user 
            var getUsers = _userService.GetAll().Where(x => x.UserType > 1).ToList(); 

            bool isNew = true;

            var thisUser = getUsers.Where(x => x.Email == practice.GP1Email).ToList();

            if (thisUser.Count() > 0)
            {
                isNew = false;
            }

            if (isNew)
            {
                Users newUser = new Users();

                string firstName = "Firstname";
                string surname = "Surname";

                string[] getNames = practice.GP1.Split(' ').ToArray();

                if (getNames.Length >= 2)
                {
                    firstName = getNames[0].ToString();
                    surname = getNames[1].ToString();
                }

                newUser.Firstname = firstName;
                newUser.Surname = surname;
                newUser.Email = practice.GP1Email;
                newUser.Username = practice.GP1Email;
                newUser.Pwd = GeneratePassword();
                newUser.UserType = 2;
                newUser.PracticeId = practice.Id;
                newUser.Year2 = false;
                newUser.Year3 = false;
                newUser.Year4 = false;
                newUser.Year5 = false;
                newUser.IsActive = true;
                newUser.DateCreated = DateTime.Now;
                newUser.DateUpdated = DateTime.Now;
                newUser.UpdatedBy = Convert.ToInt32(Session["UserId"].ToString());

                _userService.AddUser(newUser);
            }

            return isNew;

        }

        public bool CreateGPUser(PracticesExternal practice)
        {
            //create user 
            var getUsers = _userService.GetAll().Where(x => x.UserType > 1).ToList();

            bool isNew = true;

            var thisUser = getUsers.Where(x => x.Email == practice.GP1Email).ToList();

            if (thisUser.Count() > 0)
            {
                isNew = false;
            }

            if (isNew)
            {
                Users newUser = new Users();

                string firstName = "";
                string surname = "";

                string[] getNames = practice.GP1.Split(' ').ToArray();

                if (getNames.Length >= 2)
                {
                    firstName = getNames[0].ToString();
                    surname = getNames[1].ToString();
                }

                newUser.Firstname = firstName;
                newUser.Surname = surname;
                newUser.Email = practice.GP1Email;
                newUser.Username = practice.GP1Email;
                newUser.Pwd = GeneratePassword();
                newUser.UserType = 2;
                newUser.PracticeId = practice.Id;
                newUser.Year2 = false;
                newUser.Year3 = false;
                newUser.Year4 = false;
                newUser.Year5 = false;
                newUser.IsActive = true;
                newUser.DateCreated = DateTime.Now;
                newUser.DateUpdated = DateTime.Now;
                newUser.UpdatedBy = Convert.ToInt32(Session["UserId"].ToString());

                _userService.AddUser(newUser);
            }

            return isNew;

        }

        public void GenerateUsersFromExistingPractices()
        {
            var getNonArchivedPractices = _practiceService.GetAll().Where(x => x.Active == 1 || x.Queried == 1).ToList();
            var getPracticesWithEmails = getNonArchivedPractices.Where(x => x.PMEmail != "" || x.GP1Email != "").ToList();

            foreach (var practice in getPracticesWithEmails)
            {
                if (practice.PMEmail != "")
                {
                    CreatePMUser(practice);
                }

                if (practice.GP1Email != "")
                {
                    CreateGPUser(practice);
                }
            }
        }

        public ActionResult ApprovePracticeChanges(int id)
        {
            var academicYear = AcademicYearDD();

            var changedPractice = _practiceExternalService.GetById(id);
            var currentPractice = _practiceService.GetById(changedPractice.PrimaryId);

            ApprovePracticeChangesViewModel bothModels = new ApprovePracticeChangesViewModel();

            currentPractice.PracticeStatusGroup = ManagePracticeStatusGroupGET(currentPractice);
            changedPractice.PracticeStatusGroup = ManagePracticeStatusGroupGET(currentPractice, changedPractice);

            bothModels.originalRecord = currentPractice;
            bothModels.changedRecord = changedPractice;

            return View(bothModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApprovePracticeChanges(ApprovePracticeChangesViewModel approvePracticeChangesViewModel)
        {
            approvePracticeChangesViewModel.originalRecord = _practiceService.GetById(approvePracticeChangesViewModel.originalRecord.Id);


            //ignore these model state errors
            if (ModelState.ContainsKey("originalRecord.Surgery"))
            {
                ModelState["originalRecord.Surgery"].Errors.Clear();
            }

            if (ModelState.ContainsKey("originalRecord.GP1"))
            {
                ModelState["originalRecord.GP1"].Errors.Clear();
            }

            if (ModelState.ContainsKey("originalRecord.Address1"))
            {
                ModelState["originalRecord.Address1"].Errors.Clear();
            }

            if (ModelState.ContainsKey("originalRecord.Town"))
            {
                ModelState["originalRecord.Town"].Errors.Clear();
            }

            if (ModelState.ContainsKey("originalRecord.Postcode"))
            {
                ModelState["originalRecord.Postcode"].Errors.Clear();
            }

            if (ModelState.ContainsKey("originalRecord.Telephone"))
            {
                ModelState["originalRecord.Telephone"].Errors.Clear();
            }

            if (ModelState.ContainsKey("originalRecord.PracticeManager"))
            {
                ModelState["originalRecord.PracticeManager"].Errors.Clear();
            }

            if (ModelState.ContainsKey("originalRecord.PMEmail"))
            {
                ModelState["originalRecord.PMEmail"].Errors.Clear();
            }

            if (ModelState.IsValid)
            {
                var updatedPractice = ParseApprovePracticesViewModel(approvePracticeChangesViewModel);

                 //update original practice record
                updatedPractice.DateUpdated = DateTime.Now;
                updatedPractice.UpdatedBy = Convert.ToInt32(Session["UserId"].ToString());

                _practiceService.EditPractice(updatedPractice);

                //update practiceexternal record and mark as approved
                var practiceExternal = approvePracticeChangesViewModel.changedRecord;
                practiceExternal.DateApproved = DateTime.Now;
                practiceExternal.ApprovedBy = Convert.ToInt32(Session["UserId"].ToString());
                practiceExternal.ChangesApproved = true;

                _practiceExternalService.EditPractice(practiceExternal);


                return RedirectToAction("ManagePracticesExternal");
            }
            else
            {
                var academicYear = AcademicYearDD();

                return View(approvePracticeChangesViewModel);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditPracticeExternal(Practices practice, FormCollection fc)
        {
            //var myPractice = _practiceService.GetById(practice.Id);

            string guid = fc["guid"].ToString();

            if (ModelState.IsValid)
            {
                ManagePracticeStatusGroupPOST(practice);

                PracticesExternal practicesExternal = ParsePracticeToExternal(practice);

                practicesExternal.RequestedBy = Convert.ToInt32(Session["UserId"].ToString());
                practicesExternal.DateRequested = DateTime.Now;
                practicesExternal.ChangesApproved = false;

                _practiceExternalService.AddPractice(practicesExternal);

                //update signupsendlog if edit orginates from signup email
                if (!String.IsNullOrEmpty(guid))
                {
                    var myRecord = _signupSendLogService.GetByGuid(guid);

                    if (myRecord != null)
                    {
                        myRecord.DetailsUpdated = true;
                        myRecord.DateActionTaken = DateTime.Now;

                        _signupSendLogService.EditSignupSendLog(myRecord);
                    }
                }

                return RedirectToAction("PracticeUpdatedThanks");
            }
            else
            {
                return View(practice);
            }
        }

        public ActionResult PracticeUpdatedThanks()
        {
            return View();
        }

        public ActionResult EditPractice(int id)
        {
            var academicYear = AcademicYearDD();

            var myPractice = _practiceService.GetById(id);

            myPractice.PracticeStatusGroup = ManagePracticeStatusGroupGET(myPractice);

            return View(myPractice);
        }

        [HttpPost]
        public ActionResult EditPractice(Practices practice)
        {
            var myPractice = _practiceService.GetById(practice.Id);

            if (ModelState.IsValid)
            {
                ManagePracticeStatusGroupPOST(practice);

                practice.DateUpdated = DateTime.Now;
                practice.UpdatedBy = 1;
                _practiceService.EditPractice(practice);

                return RedirectToAction("ManagePractices");
            }
            else
            {
                return View(myPractice);
            }
        }

        public ActionResult ManagePractices(int practiceStatus = 0)
        {
            List<Practices> myPractices = _practiceService.GetAll();

            switch (practiceStatus)
            {
                case 1:
                    myPractices = myPractices.Where(x => x.Active == 1).ToList();
                    break;

                case 2:
                    myPractices = myPractices.Where(x => x.Queried == 1).ToList();
                    break;

                case 3:
                    myPractices = myPractices.Where(x => x.Disabled == 1).ToList();
                    break;
            }

            return View(myPractices);
        }

        public ActionResult ManageNotReturned()
        {
            var academicYear = AcademicYearDD();
            List<Practices> notReturned = _practiceService.GetPracticesNotReturnedSignup(academicYear);

            return View(notReturned);
        }

        public ActionResult ManageReturned()
        {
            var academicYear = AcademicYearDD();
            List<Practices> Returned = _practiceService.GetPracticesReturnedSignup(academicYear);

            return View(Returned);
        }

        public ActionResult ManagePracticesExternal()
        {
            List<PracticesExternal> myPractices = _practiceExternalService.GetAllPending();

            return View(myPractices);
        }

        public ActionResult ManageSignupReturns()
        {
            var academicYear = AcademicYearDD();

            //var getReturns = _signupSendLogService.GetAllNoActivity(academicYear).Where(x => x.DetailsUpdated == true).OrderByDescending(x => x.DateActionTaken).ToList();
            var getReturns = _allocationService.GetByAcademicYear(academicYear).Where(x => x.AllocationApproved == false).OrderByDescending(x => x.DateCreated).ToList();

            List<Practices> PracticesReturned = new List<Practices>();

            var getAllPractices = _practiceService.GetAll();

            foreach (var signup in getReturns)
            {
                var getPractice = getAllPractices.Where(x => x.Id == signup.PracticeId).FirstOrDefault();

                PracticesReturned.Add(getPractice);
            }

            return View(PracticesReturned);
        }

        public ActionResult AddAllocation(int id)
        {
            var academicYear = AcademicYearDD();

            //check if allocation exists for PracticeId AND academic year
            var doesAllocationExist = _allocationService.GetByPracticeAndYear(id, academicYear);

            if (doesAllocationExist != null)
            {
                return RedirectToAction("EditAllocation", new { id = doesAllocationExist.Id });
            }


            return View(BuildAddAllocation(id));
        }

        private AllocationViewModel BuildAddAllocation(int PracticeId)
        {
            var myPractice = _practiceService.GetById(PracticeId);

            AllocationViewModel allocationViewModel = new AllocationViewModel();

            allocationViewModel.PracticeId = myPractice.Id;
            allocationViewModel.Surgery = myPractice.Surgery;
            allocationViewModel.Postcode = myPractice.Postcode;
            allocationViewModel.Notes = myPractice.Notes;

            return allocationViewModel;
        }

        public ActionResult EditAllocation(int id)
        {
            var academicYear = AcademicYearDD();

            var myAllocation = _allocationService.GetById(id);

            AllocationViewModel allocationViewModel = BuildAddAllocation(myAllocation.PracticeId);
            List<Practices> myPractices = _practiceService.GetAll();

            allocationViewModel = ParseAllocationViewModelEDIT(allocationViewModel, myAllocation, myPractices);

            return View(allocationViewModel);
        }

        [HttpPost]
        public ActionResult EditAllocation(AllocationViewModel allocationViewModel)
        {
            var myAllocation = _allocationService.GetById(allocationViewModel.AllocationId);

            if (ModelState.IsValid)
            {
                allocationViewModel.DateUpdated = DateTime.Now;
                myAllocation.AllocationApproved = true;

                var updateAllocation = _allocationService.EditAllocation(ParseAllocationViewModelADD(allocationViewModel, myAllocation));

                return RedirectToAction("ManagePractices");
            }
            else
            {
                return View(allocationViewModel);
            }
        }

        [HttpPost]
        public ActionResult AddAllocation(AllocationViewModel allocationViewModel)
        {
            Allocations allocation = new Allocations();

            if (ModelState.IsValid)
            {
                allocation.DateCreated = DateTime.Now;
                allocation.UpdatedBy = Convert.ToInt32(Session["UserId"].ToString());                

                var myAllocation = _allocationService.AddAllocation(ParseAllocationViewModelADD(allocationViewModel, allocation));

                return RedirectToAction("ManagePractices");
            }
            else
            {

            }
            return View(BuildAddAllocation(allocationViewModel.PracticeId));
        }

        public ActionResult AtAGlance()
        {
            var academicYear = AcademicYearDD();

            var myAllocation = _allocationService.GetByAcademicYear(academicYear).Where(x => x.AllocationApproved == true);
            var myPractices = _practiceService.GetAll();

            List<AllocationViewModel> allocationViewModel = new List<AllocationViewModel>();

            foreach (var allocation in myAllocation)
            {
                allocationViewModel.Add(ParseAllocationViewModelEDIT(new AllocationViewModel(), allocation, myPractices));

            }

            return View(allocationViewModel);
        }

        public ActionResult AtAGlanceRequested()
        {
            var academicYear = AcademicYearDD();

            var myAllocation = _allocationService.GetByAcademicYear(academicYear);
            var myPractices = _practiceService.GetAll();

            List<AllocationViewModel> allocationViewModel = new List<AllocationViewModel>();

            foreach (var allocation in myAllocation)
            {
                allocationViewModel.Add(ParseAllocationViewModelEDIT(new AllocationViewModel(), allocation, myPractices));

            }

            return View(allocationViewModel);
        }

        public ActionResult ManageUsers()
        {
            var myUsers = _userService.GetAll();

            List<UserViewModel> userViewModel = new List<UserViewModel>(0);

            foreach (var user in myUsers)
            {
                userViewModel.Add(new UserViewModel
                {
                    Firstname = user.Firstname,
                    Surname = user.Surname,
                    Email = user.Email,
                    UserType = Enum.GetName(typeof(UserTypes), user.UserType).ToString(),
                    IsActive = user.IsActive,
                    Id = user.Id
                });
            }

            return View(userViewModel);
        }

        public ActionResult EditUser(int id)
        {
            var myUser = _userService.GetById(id);

            PrepareUserViewBag();

            return View(myUser);
        }

        [HttpPost]
        public ActionResult EditUser(Users user)
        {
            var myUser = _userService.GetById(user.Id);

            if (ModelState.IsValid)
            {
                myUser.Firstname = user.Firstname;
                myUser.Surname = user.Surname;
                myUser.Email = user.Email;
                myUser.Username = user.Username;
                myUser.UserType = user.UserType;
                myUser.PracticeId = user.PracticeId;

                //myUser.Pwd = user.Pwd;
                myUser.IsActive = user.IsActive;
                myUser.DateUpdated = DateTime.Now;
                //myUser.UpdatedBy = Convert.ToInt32(Session["UserId"].ToString());

                _userService.EditUser(myUser);

                return RedirectToAction("ManageUsers", "Home");
            }
            else
            {
                return View(myUser);
            }
        }

        public ActionResult AddUser()
        {
            PrepareUserViewBag();


            return View();
        }

        [HttpPost]
        public ActionResult AddUser(Users user)
        {
            if (user.UserType == (int)UserTypes.Practice && user.PracticeId == 0)
            {
                ModelState.AddModelError("practiceid", "Please select the correct practice for this user");
            }

            if (user.UserType == (int)UserTypes.Practice)
            {
                if (user.Username != user.Email)
                {
                    ModelState.AddModelError("username", "Please ensure that username and email address match for Practice users");
                }
            }


            if (ModelState.IsValid)
            {
                Users myUser = new Users();

                myUser.Firstname = user.Firstname;
                myUser.Surname = user.Surname;
                myUser.Email = user.Email;
                myUser.Username = user.Username;
                myUser.UserType = user.UserType;
                myUser.PracticeId = user.PracticeId;

                //Password generation only required for external/practice users
                if (user.UserType == (int)UserTypes.Practice)
                {
                    myUser.Pwd = GeneratePassword();
                }

                myUser.IsActive = user.IsActive;
                myUser.DateCreated = DateTime.Now;
                //myUser.UpdatedBy = Convert.ToInt32(Session["UserId"].ToString());

                _userService.AddUser(myUser);

                return RedirectToAction("ManageUsers", "Home");
            }
            else
            {
                PrepareUserViewBag();

                return View();
            }
        }

        public ActionResult SendSignUpInvite()
        {
            var getTemplate = _emailTemplateService.GetById((int)EmailTypes.SignUpInvite);

            ViewBag.SendTypes = SignupEmailSendTypes();

            return View(getTemplate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendSignUpInvite(EmailTemplates emailTemplates, HttpPostedFileBase attachmentFile, HttpPostedFileBase attachmentFile2, string Command, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var myEmail = _emailTemplateService.GetById(emailTemplates.Id);

                //remove reference to file attachment then delete file from file system
                if (fc["removeAttachment"] != null)
                {
                    emailTemplates.AttachmentName = null;

                    DeleteAttachment(myEmail.AttachmentName);
                }

                if (fc["removeAttachment2"] != null)
                {
                    emailTemplates.AttachmentName2 = null;

                    DeleteAttachment(myEmail.AttachmentName2);
                }

                myEmail = emailTemplates;

                if (attachmentFile != null)
                {
                    myEmail.AttachmentName = UploadDocument(attachmentFile);
                }

                if (attachmentFile2 != null)
                {
                    myEmail.AttachmentName2 = UploadDocument(attachmentFile2);
                }

                myEmail.DateUpdated = DateTime.Now;
                myEmail.UpdatedBy = Convert.ToInt32(Session["UserId"].ToString());

                _emailTemplateService.EditEmailTemplate(myEmail);

                List<Users> Sendlist = new List<Users>(0);


                Sendlist = GetSendListByPracticeStatus(emailTemplates.SendList);

                //switch (emailTemplates.SendList)
                //{
                //    case 1:
                //        Sendlist = _userService.GetAllPracticeUsers();
                //        break;

                //    case 2:
                //        Sendlist = GetUsersFromSendListLog();
                //        break;
                //}


                if (Command == "Send Preview Email")
                {
                    var myAdmin = _userService.GetById(Convert.ToInt32(Session["UserId"].ToString()));

                    BuildEmail(myAdmin, null, myEmail);
                    logger.Info("Preview email sent: " + myAdmin.Email);
                    return this.RedirectToAction("SendSignUpInvite", "Home");
                }
                else
                {
                    
                    return RedirectToAction("InviteSent", "Home", new {getSendCode = BuildEmail(null, Sendlist, myEmail) });
                }

                
            }
            else
            {
                ViewBag.SendTypes = SignupEmailSendTypes();

                return View(emailTemplates);

            }

        }

        public List<Users> GetSendListByPracticeStatus(int practiceStatus)
        {
            List<Users> sendList = new List<Users>(0);
            List<Users> getUsers = _userService.GetAllPracticeUsers();

            var getPractices = _practiceService.GetAll();

            switch (practiceStatus)
            {
                case 1:
                    getPractices = getPractices.Where(x => x.Active ==1).ToList();
                    break;

                case 2:
                    getPractices = getPractices.Where(x => x.Queried == 1).ToList();
                    break;

                case 3:
                    getPractices = getPractices.Where(x => x.Disabled == 1).ToList();
                    break;
            }

            foreach (var practice in getPractices)
            {
                List<Users> practiceUsers = getUsers.Where(x => x.PracticeId == practice.Id).ToList();

                foreach (var user in practiceUsers)
                {
                    sendList.Add(user);
                }
            }

            return sendList;
        }

        public ActionResult InviteSent(string getSendCode)
        {
            //TODO - show list/count of emails sent?
            ViewData["SendCount"] = _signupSendLogService.GetBySendCode(getSendCode).Count();

            return View();
        }

        public List<Users> GetUsersFromSendListLog()
        {
            List<Users> myUsers = new List<Users>();
            List<Users> getUsers = _userService.GetAllPracticeUsers();

            var getSendLog = _signupSendLogService.GetAllNoActivity(AcademicYearDD());

            foreach (var user in getUsers)
            {
                //list all entries for practice. If they don't respond then multiple entries are possible in the send log
                var sendList = getSendLog.Where(x => x.PracticeId == user.PracticeId);

                bool addToMyUser = true;

                //if any of the entries are true for the academic year then the sign up invite has been actioned. Other updates to practice details can be managed manually at any time by the practice user logging in
                foreach (var listItem in sendList)
                {
                    if (listItem.NoChangesClicked)
                    {
                        addToMyUser = false;
                    }

                    if (listItem.DetailsUpdated)
                    {
                        addToMyUser = false;
                    }
                }

                if (addToMyUser)
                {
                    myUsers.Add(user);
                }
            }

            return myUsers;
        }

        public string UploadDocument(HttpPostedFileBase fileToUpload)
        {
            string uploadFolder = Server.MapPath(getAttachmentPath);
            string guid = Guid.NewGuid().ToString();

            string fileType = Path.GetExtension(fileToUpload.FileName);
            string fileName = Path.GetFileNameWithoutExtension(fileToUpload.FileName);
            string uniqueFileName = fileName + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + fileType;
            string uniqueFilePath = Path.Combine(uploadFolder, uniqueFileName);

            if (System.IO.File.Exists(uniqueFilePath))
            {
                uniqueFileName = guid + "-" + fileName + fileType;
                uniqueFilePath = Path.Combine(uploadFolder, uniqueFileName);
            }

            fileToUpload.SaveAs(uniqueFilePath);

            return uniqueFileName;
        }

        public void DeleteAttachment(string atttachmentName)
        {
            string fileLocation = Server.MapPath(getAttachmentPath) + atttachmentName;
            if (System.IO.File.Exists(fileLocation))
            {
                System.IO.File.Delete(fileLocation);
            }
        }

        public List<SelectListItem> SignupEmailSendTypes()
        {
            List<SelectListItem> li = new List<SelectListItem>();

            li.Add(new SelectListItem { Text = "Select", Value = "" });
            li.Add(new SelectListItem { Text = "Active practices", Value = "1" });
            li.Add(new SelectListItem { Text = "Dormant practices", Value = "2" });
            li.Add(new SelectListItem { Text = "Archived practices", Value = "3" });

            return li;
        }

        public void PrepareUserViewBag()
        {
            ViewBag.Practices = _practiceService.GetAll().ToSelectList(nameof(Practices.Id), nameof(Practices.Surgery));
            ViewBag.UserTypes = EnumToList<UserTypes>().ToSelectList(nameof(MyEnumModel.Id), nameof(MyEnumModel.Value));
        }

        public List<MyEnumModel> EnumToList<T>()
        {
            List<MyEnumModel> myTypes = new List<MyEnumModel>(0);

            foreach (var value in Enum.GetValues(typeof(T)))
            {
                myTypes.Add(new MyEnumModel
                {
                    Id = (int)value,
                    Value = value.ToString()
                });
            }

            return myTypes;
        }



        private Allocations ParseAllocationViewModelADD(AllocationViewModel allocationViewModel, Allocations allocation)
        {
            allocation.PracticeId = allocationViewModel.PracticeId;
            allocation.Year2Wk1Requested = allocationViewModel.Year2Wk1Requested;
            allocation.Year2Wk1Allocated = allocationViewModel.Year2Wk1Allocated;
            allocation.Year2Wk2Requested = allocationViewModel.Year2Wk2Requested;
            allocation.Year2Wk2Allocated = allocationViewModel.Year2Wk2Allocated;
            allocation.Year2Wk3Requested = allocationViewModel.Year2Wk3Requested;
            allocation.Year2Wk3Allocated = allocationViewModel.Year2Wk3Allocated;
            allocation.Year2Wk4Requested = allocationViewModel.Year2Wk4Requested;
            allocation.Year2Wk4Allocated = allocationViewModel.Year2Wk4Allocated;
            allocation.Year2Wk5Requested = allocationViewModel.Year2Wk5Requested;
            allocation.Year2Wk5Allocated = allocationViewModel.Year2Wk5Allocated;
            allocation.Year2Wk6Requested = allocationViewModel.Year2Wk6Requested;
            allocation.Year2Wk6Allocated = allocationViewModel.Year2Wk6Allocated;
            allocation.Year3B1Requested = allocationViewModel.Year3B1Requested;
            allocation.Year3B1Allocated = allocationViewModel.Year3B1Allocated;
            allocation.Year3B2Requested = allocationViewModel.Year3B2Requested;
            allocation.Year3B2Allocated = allocationViewModel.Year3B2Allocated;
            allocation.Year3B3Requested = allocationViewModel.Year3B3Requested;
            allocation.Year3B3Allocated = allocationViewModel.Year3B3Allocated;
            allocation.Year3B4Requested = allocationViewModel.Year3B4Requested;
            allocation.Year3B4Allocated = allocationViewModel.Year3B4Allocated;
            allocation.Year3B5Requested = allocationViewModel.Year3B5Requested;
            allocation.Year3B5Allocated = allocationViewModel.Year3B5Allocated;
            allocation.Year3B6Requested = allocationViewModel.Year3B6Requested;
            allocation.Year3B6Allocated = allocationViewModel.Year3B6Allocated;
            allocation.Year3B7Requested = allocationViewModel.Year3B7Requested;
            allocation.Year3B7Allocated = allocationViewModel.Year3B7Allocated;
            allocation.Year4B1Requested = allocationViewModel.Year4B1Requested;
            allocation.Year4B1Allocated = allocationViewModel.Year4B1Allocated;
            allocation.Year4B2Requested = allocationViewModel.Year4B2Requested;
            allocation.Year4B2Allocated = allocationViewModel.Year4B2Allocated;
            allocation.Year4B3Requested = allocationViewModel.Year4B3Requested;
            allocation.Year4B3Allocated = allocationViewModel.Year4B3Allocated;
            allocation.Year4B4Requested = allocationViewModel.Year4B4Requested;
            allocation.Year4B4Allocated = allocationViewModel.Year4B4Allocated;
            allocation.Year4B5Requested = allocationViewModel.Year4B5Requested;
            allocation.Year4B5Allocated = allocationViewModel.Year4B5Allocated;
            allocation.Year4B6Requested = allocationViewModel.Year4B6Requested;
            allocation.Year4B6Allocated = allocationViewModel.Year4B6Allocated;
            allocation.Year4B7Requested = allocationViewModel.Year4B7Requested;
            allocation.Year4B7Allocated = allocationViewModel.Year4B7Allocated;
            allocation.Year4B8Requested = allocationViewModel.Year4B8Requested;
            allocation.Year4B8Allocated = allocationViewModel.Year4B8Allocated;
            allocation.Year5B1Requested = allocationViewModel.Year5B1Requested;
            allocation.Year5B1Allocated = allocationViewModel.Year5B2Allocated;
            allocation.Year5B2Requested = allocationViewModel.Year5B2Requested;
            allocation.Year5B2Allocated = allocationViewModel.Year5B2Allocated;
            allocation.Year5B3Requested = allocationViewModel.Year5B3Requested;
            allocation.Year5B3Allocated = allocationViewModel.Year5B3Allocated;
            allocation.Year5B4Requested = allocationViewModel.Year5B4Requested;
            allocation.Year5B4Allocated = allocationViewModel.Year5B4Allocated;
            allocation.Year5B5Requested = allocationViewModel.Year5B5Requested;
            allocation.Year5B5Allocated = allocationViewModel.Year5B5Allocated;
            allocation.Year5B6Requested = allocationViewModel.Year5B6Requested;
            allocation.Year5B6Allocated = allocationViewModel.Year5B6Allocated;
            allocation.AcademicYear = allocationViewModel.AcademicYear;
            allocation.ServiceContractReceived = allocationViewModel.ServiceContractReceived;
            //allocation.DateCreated = allocationViewModel.DateCreated;
            //allocation.DateUpdated = allocationViewModel.DateUpdated;
            //allocation.UpdatedBy = allocationViewModel.UpdatedBy;

            return allocation;
        }

        private AllocationViewModel ParseAllocationViewModelEDIT(AllocationViewModel allocationViewModel, Allocations myAllocation, List<Practices> myPractices)
        {
            var getPractice = myPractices.Where(x => x.Id == myAllocation.PracticeId);

            if (getPractice != null)
            {
                allocationViewModel.Surgery = getPractice.FirstOrDefault().Surgery;
                allocationViewModel.Postcode = getPractice.FirstOrDefault().Postcode;
            }

            allocationViewModel.AllocationId = myAllocation.Id;
            allocationViewModel.PracticeId = myAllocation.PracticeId;
            allocationViewModel.Year2Wk1Requested = myAllocation.Year2Wk1Requested;
            allocationViewModel.Year2Wk1Allocated = myAllocation.Year2Wk1Allocated;
            allocationViewModel.Year2Wk2Requested = myAllocation.Year2Wk2Requested;
            allocationViewModel.Year2Wk2Allocated = myAllocation.Year2Wk2Allocated;
            allocationViewModel.Year2Wk3Requested = myAllocation.Year2Wk3Requested;
            allocationViewModel.Year2Wk3Allocated = myAllocation.Year2Wk3Allocated;
            allocationViewModel.Year2Wk4Requested = myAllocation.Year2Wk4Requested;
            allocationViewModel.Year2Wk4Allocated = myAllocation.Year2Wk4Allocated;
            allocationViewModel.Year2Wk5Requested = myAllocation.Year2Wk5Requested;
            allocationViewModel.Year2Wk5Allocated = myAllocation.Year2Wk5Allocated;
            allocationViewModel.Year2Wk6Requested = myAllocation.Year2Wk6Requested;
            allocationViewModel.Year2Wk6Allocated = myAllocation.Year2Wk6Allocated;
            allocationViewModel.Year3B1Requested = myAllocation.Year3B1Requested;
            allocationViewModel.Year3B1Allocated = myAllocation.Year3B1Allocated;
            allocationViewModel.Year3B2Requested = myAllocation.Year3B2Requested;
            allocationViewModel.Year3B2Allocated = myAllocation.Year3B2Allocated;
            allocationViewModel.Year3B3Requested = myAllocation.Year3B3Requested;
            allocationViewModel.Year3B3Allocated = myAllocation.Year3B3Allocated;
            allocationViewModel.Year3B4Requested = myAllocation.Year3B4Requested;
            allocationViewModel.Year3B4Allocated = myAllocation.Year3B4Allocated;
            allocationViewModel.Year3B5Requested = myAllocation.Year3B5Requested;
            allocationViewModel.Year3B5Allocated = myAllocation.Year3B5Allocated;
            allocationViewModel.Year3B6Requested = myAllocation.Year3B6Requested;
            allocationViewModel.Year3B6Allocated = myAllocation.Year3B6Allocated;
            allocationViewModel.Year3B7Requested = myAllocation.Year3B7Requested;
            allocationViewModel.Year3B7Allocated = myAllocation.Year3B7Allocated;
            allocationViewModel.Year4B1Requested = myAllocation.Year4B1Requested;
            allocationViewModel.Year4B1Allocated = myAllocation.Year4B1Allocated;
            allocationViewModel.Year4B2Requested = myAllocation.Year4B2Requested;
            allocationViewModel.Year4B2Allocated = myAllocation.Year4B2Allocated;
            allocationViewModel.Year4B3Requested = myAllocation.Year4B3Requested;
            allocationViewModel.Year4B3Allocated = myAllocation.Year4B3Allocated;
            allocationViewModel.Year4B4Requested = myAllocation.Year4B4Requested;
            allocationViewModel.Year4B4Allocated = myAllocation.Year4B4Allocated;
            allocationViewModel.Year4B5Requested = myAllocation.Year4B5Requested;
            allocationViewModel.Year4B5Allocated = myAllocation.Year4B5Allocated;
            allocationViewModel.Year4B6Requested = myAllocation.Year4B6Requested;
            allocationViewModel.Year4B6Allocated = myAllocation.Year4B6Allocated;
            allocationViewModel.Year4B7Requested = myAllocation.Year4B7Requested;
            allocationViewModel.Year4B7Allocated = myAllocation.Year4B7Allocated;
            allocationViewModel.Year4B8Requested = myAllocation.Year4B8Requested;
            allocationViewModel.Year4B8Allocated = myAllocation.Year4B8Allocated;
            allocationViewModel.Year5B1Requested = myAllocation.Year5B1Requested;
            allocationViewModel.Year5B1Allocated = myAllocation.Year5B2Allocated;
            allocationViewModel.Year5B2Requested = myAllocation.Year5B2Requested;
            allocationViewModel.Year5B2Allocated = myAllocation.Year5B2Allocated;
            allocationViewModel.Year5B3Requested = myAllocation.Year5B3Requested;
            allocationViewModel.Year5B3Allocated = myAllocation.Year5B3Allocated;
            allocationViewModel.Year5B4Requested = myAllocation.Year5B4Requested;
            allocationViewModel.Year5B4Allocated = myAllocation.Year5B4Allocated;
            allocationViewModel.Year5B5Requested = myAllocation.Year5B5Requested;
            allocationViewModel.Year5B5Allocated = myAllocation.Year5B5Allocated;
            allocationViewModel.Year5B6Requested = myAllocation.Year5B6Requested;
            allocationViewModel.Year5B6Allocated = myAllocation.Year5B6Allocated;
            allocationViewModel.AcademicYear = myAllocation.AcademicYear;
            allocationViewModel.ServiceContractReceived = myAllocation.ServiceContractReceived;

            return allocationViewModel;
        }

        private Practices ParseApprovePracticesViewModel(ApprovePracticeChangesViewModel approvePracticeChangesViewModel)
        {
            approvePracticeChangesViewModel.originalRecord.Surgery = approvePracticeChangesViewModel.changedRecord.Surgery;

            //approvePracticeChangesViewModel.originalRecord.SurgeryInUse = approvePracticeChangesViewModel.changedRecord.SurgeryInUse;
            approvePracticeChangesViewModel.originalRecord.GP1 = approvePracticeChangesViewModel.changedRecord.GP1;
            approvePracticeChangesViewModel.originalRecord.GP1Email = approvePracticeChangesViewModel.changedRecord.GP1Email;
            approvePracticeChangesViewModel.originalRecord.Address1 = approvePracticeChangesViewModel.changedRecord.Address1;
            approvePracticeChangesViewModel.originalRecord.Address2 = approvePracticeChangesViewModel.changedRecord.Address2;
            approvePracticeChangesViewModel.originalRecord.Town = approvePracticeChangesViewModel.changedRecord.Town;
            approvePracticeChangesViewModel.originalRecord.Postcode = approvePracticeChangesViewModel.changedRecord.Postcode;
            approvePracticeChangesViewModel.originalRecord.Telephone = approvePracticeChangesViewModel.changedRecord.Telephone;
            //approvePracticeChangesViewModel.originalRecord.Fax = approvePracticeChangesViewModel.changedRecord.Fax;
            approvePracticeChangesViewModel.originalRecord.PracticeManager = approvePracticeChangesViewModel.changedRecord.PracticeManager;
            approvePracticeChangesViewModel.originalRecord.PMEmail = approvePracticeChangesViewModel.changedRecord.PMEmail;
            approvePracticeChangesViewModel.originalRecord.GP2 = approvePracticeChangesViewModel.changedRecord.GP2;
            approvePracticeChangesViewModel.originalRecord.GP2Email = approvePracticeChangesViewModel.changedRecord.GP2Email;
            //approvePracticeChangesViewModel.originalRecord.Website = approvePracticeChangesViewModel.changedRecord.Website;
            //approvePracticeChangesViewModel.originalRecord.GP3 = approvePracticeChangesViewModel.changedRecord.GP3;
            //approvePracticeChangesViewModel.originalRecord.GP3Email = approvePracticeChangesViewModel.changedRecord.GP3Email;
            //approvePracticeChangesViewModel.originalRecord.GP4 = approvePracticeChangesViewModel.changedRecord.GP4;
            //approvePracticeChangesViewModel.originalRecord.GP4Email = approvePracticeChangesViewModel.changedRecord.GP4Email;
            approvePracticeChangesViewModel.originalRecord.AdditionalEmails = approvePracticeChangesViewModel.changedRecord.AdditionalEmails;
            approvePracticeChangesViewModel.originalRecord.SupplierNumber = approvePracticeChangesViewModel.changedRecord.SupplierNumber;
            //approvePracticeChangesViewModel.originalRecord.ContactSurgery = approvePracticeChangesViewModel.changedRecord.ContactSurgery;
            //approvePracticeChangesViewModel.originalRecord.Notes = approvePracticeChangesViewModel.changedRecord.Notes;
            //approvePracticeChangesViewModel.originalRecord.AttachmentsAllocated = approvePracticeChangesViewModel.changedRecord.AttachmentsAllocated;
            //approvePracticeChangesViewModel.originalRecord.UCCTNotes = approvePracticeChangesViewModel.changedRecord.UCCTNotes;
            //approvePracticeChangesViewModel.originalRecord.QualityVisitDateR1 = approvePracticeChangesViewModel.changedRecord.QualityVisitDateR1;
            //approvePracticeChangesViewModel.originalRecord.QualityVisitNotes = approvePracticeChangesViewModel.changedRecord.QualityVisitNotes;
            //approvePracticeChangesViewModel.originalRecord.Active = approvePracticeChangesViewModel.changedRecord.Active;
            //approvePracticeChangesViewModel.originalRecord.Disabled = approvePracticeChangesViewModel.changedRecord.Disabled;
            //approvePracticeChangesViewModel.originalRecord.Queried = approvePracticeChangesViewModel.changedRecord.Queried;
            approvePracticeChangesViewModel.originalRecord.ListSize = approvePracticeChangesViewModel.changedRecord.ListSize;
            //approvePracticeChangesViewModel.originalRecord.NewPractice = approvePracticeChangesViewModel.changedRecord.NewPractice;
            //approvePracticeChangesViewModel.originalRecord.AcademicYear = approvePracticeChangesViewModel.changedRecord.AcademicYear;
            //approvePracticeChangesViewModel.originalRecord.QualityVisitDate = approvePracticeChangesViewModel.changedRecord.QualityVisitDate;
            //approvePracticeChangesViewModel.originalRecord.OKToProceed = approvePracticeChangesViewModel.changedRecord.OKToProceed;
            //approvePracticeChangesViewModel.originalRecord.DataReviewDate = approvePracticeChangesViewModel.changedRecord.DataReviewDate;
            //approvePracticeChangesViewModel.originalRecord.TutorTrainingGPName = approvePracticeChangesViewModel.changedRecord.TutorTrainingGPName;
            //approvePracticeChangesViewModel.originalRecord.TutorTrainingDate = approvePracticeChangesViewModel.changedRecord.TutorTrainingDate;

            return approvePracticeChangesViewModel.originalRecord;
        }

        private Practices ParsePracticesExternalForRegistration(PracticesExternal practices)
        {
            Practices newPractice = new Practices();

            newPractice.Surgery = practices.Surgery;
            newPractice.GP1 = practices.GP1;
            newPractice.GP1Email = practices.GP1Email;
            newPractice.Address1 = practices.Address1;
            newPractice.Address2 = practices.Address2;
            newPractice.Town = practices.Town;
            newPractice.Postcode = practices.Postcode;
            newPractice.Telephone = practices.Telephone;
            newPractice.PracticeManager = practices.PracticeManager;
            newPractice.PMEmail = practices.PMEmail;
            newPractice.GP2 = practices.GP2;
            newPractice.GP2Email = practices.GP2Email;
            newPractice.AdditionalEmails = practices.AdditionalEmails;
            newPractice.SupplierNumber = practices.SupplierNumber;
            newPractice.ContactSurgery = practices.ContactSurgery;
            newPractice.Notes = practices.Notes;
            newPractice.QualityVisitNotes = practices.QualityVisitNotes;
            newPractice.Active = practices.Active;
            newPractice.Disabled = practices.Disabled;
            newPractice.Queried = practices.Queried;
            newPractice.ListSize = practices.ListSize;
            newPractice.NewPractice = practices.NewPractice;
            newPractice.AcademicYear = practices.AcademicYear;


            return newPractice;
        }

        public void DownloadAtAGlance()
        {

            CreateWorkbook();
        }

        public void DownloadAtAGlanceCombined()
        {

            CreateWorkbook();
        }

        public void DownloadPractices(int practiceStatus = 0)
        {
            CreateWorkbookPractices(practiceStatus);
        }

        public void CreateWorkbook()
        {
            List<string> wsNames = new List<string>();
            wsNames.Add("Allocated");
            wsNames.Add("Requested");

            //Create Excel object

            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();

            createWorksheet(wsNames[0].ToString(), ep);
            createWorksheetRequested(wsNames[1].ToString(), ep);

            BuildDownload("AtAGlance", ep);

        }

        public void CreateWorkbookPractices(int practiceStatus)
        {
            List<string> wsNames = new List<string>();
            wsNames.Add("Practices");

            //Create Excel object
            ExcelPackage ep = new ExcelPackage();

            createWorksheetPractices(wsNames[0].ToString(), ep, practiceStatus);

            BuildDownload("AllPractices", ep);

        }

        private void BuildDownload(string getFileName, ExcelPackage ep)
        {
            string fileName = getFileName + "-" + DateTime.Now.ToString("MM-dd-yyyy_HH-mm") + ".xlsx";

            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;  filename=" + fileName);
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();
        }

        private ExcelWorksheet createWorksheetPractices(string wsName, ExcelPackage ep, int practiceStatus)
        {
            var academicYear = AcademicYearDD();

            var myPractices = _practiceService.GetAll();

            switch (practiceStatus)
            {
                case 1:
                    myPractices = myPractices.Where(x => x.Active == 1).ToList();
                    break;

                case 2:
                    myPractices = myPractices.Where(x => x.Queried == 1).ToList();
                    break;

                case 3:
                    myPractices = myPractices.Where(x => x.Disabled == 1).ToList();
                    break;
            }

            ExcelWorksheet worksheet = ep.Workbook.Worksheets.Add(wsName);

            var format = new ExcelTextFormat();
            format.Delimiter = ';';
            format.TextQualifier = '"';
            format.DataTypes = new[] { eDataTypes.String };

            worksheet.Cells["A2"].LoadFromText("Practice Name");
            worksheet.Cells["B2"].LoadFromText("Supplier Number");
            worksheet.Cells["C2"].LoadFromText("New Practice");
            worksheet.Cells["D2"].LoadFromText("Address1");
            worksheet.Cells["E2"].LoadFromText("Address2");
            worksheet.Cells["F2"].LoadFromText("Town");
            worksheet.Cells["G2"].LoadFromText("Postcode");
            worksheet.Cells["I2"].LoadFromText("Telephone");
            worksheet.Cells["I2"].LoadFromText(GetAttributeDisplayNamePractice("PracticeManager"));
            worksheet.Cells["J2"].LoadFromText(GetAttributeDisplayNamePractice("PMEmail"));
            worksheet.Cells["K2"].LoadFromText(GetAttributeDisplayNamePractice("GP1"));
            worksheet.Cells["L2"].LoadFromText(GetAttributeDisplayNamePractice("GP1Email"));
            worksheet.Cells["M2"].LoadFromText(GetAttributeDisplayNamePractice("GP2"));
            worksheet.Cells["N2"].LoadFromText(GetAttributeDisplayNamePractice("GP2Email"));
            worksheet.Cells["O2"].LoadFromText(GetAttributeDisplayNamePractice("AdditionalEmails"));
            worksheet.Cells["P2"].LoadFromText(GetAttributeDisplayNamePractice("ListSize"));
            worksheet.Cells["Q2"].LoadFromText("Practice Status");
            worksheet.Cells["R2"].LoadFromText(GetAttributeDisplayNamePractice("TutorTrainingGPName"));
            worksheet.Cells["S2"].LoadFromText(GetAttributeDisplayNamePractice("ContactSurgery"));
            worksheet.Cells["T2"].LoadFromText(GetAttributeDisplayNamePractice("Notes"));
            worksheet.Cells["U2"].LoadFromText(GetAttributeDisplayNamePractice("QualityVisitNotes"));

            var mainHeader = worksheet.Cells["A2:U2"];
            mainHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
            Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#eaecf4");
            mainHeader.Style.Fill.BackgroundColor.SetColor(colFromHex);
            mainHeader.Style.Font.Bold = true;

            int rowCounter = 3;

            string myRange = "C" + rowCounter + ":V" + rowCounter;
            var mainCells = worksheet.Cells[myRange];

            foreach (var practice in myPractices)
            {
                myRange = "A" + rowCounter + ":V" + rowCounter;
                mainCells = worksheet.Cells[myRange];
                mainCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells[rowCounter, 1].Value = practice.Surgery;
                worksheet.Cells[rowCounter, 2].Value = practice.SupplierNumber;
                worksheet.Cells[rowCounter, 3].Value = ShowBoolean(practice.NewPractice);
                worksheet.Cells[rowCounter, 4].Value = practice.Address1;
                worksheet.Cells[rowCounter, 5].Value = practice.Address2;
                worksheet.Cells[rowCounter, 6].Value = practice.Town;
                worksheet.Cells[rowCounter, 7].Value = practice.Postcode;
                worksheet.Cells[rowCounter, 8].Value = practice.Telephone;

                worksheet.Cells[rowCounter, 9].Value = practice.PracticeManager;
                worksheet.Cells[rowCounter, 10].Value = practice.PMEmail;
                worksheet.Cells[rowCounter, 11].Value = practice.GP1;
                worksheet.Cells[rowCounter, 12].Value = practice.GP1Email;
                worksheet.Cells[rowCounter, 13].Value = practice.GP2;
                worksheet.Cells[rowCounter, 14].Value = practice.GP2Email;
                worksheet.Cells[rowCounter, 15].Value = practice.AdditionalEmails;

                worksheet.Cells[rowCounter, 16].Value = practice.ListSize;
                worksheet.Cells[rowCounter, 17].Value = ShowPracticeStatus(ManagePracticeStatusGroupGET(practice));
                worksheet.Cells[rowCounter, 18].Value = practice.TutorTrainingGPName;
                worksheet.Cells[rowCounter, 19].Value = ShowBoolean(practice.ContactSurgery);
                worksheet.Cells[rowCounter, 20].Value = practice.Notes;
                worksheet.Cells[rowCounter, 21].Value = practice.QualityVisitNotes;

                rowCounter++;
            }
                    
            return worksheet;
        }

        private ExcelWorksheet createWorksheet(string wsName, ExcelPackage ep)
        {
            var academicYear = AcademicYearDD();

            var myAllocation = _allocationService.GetByAcademicYear(academicYear).Where(x => x.AllocationApproved == true);
            var myPractices = _practiceService.GetAll();

            List<AllocationViewModel> allocationViewModel = new List<AllocationViewModel>();

            foreach (var allocation in myAllocation)
            {
                allocationViewModel.Add(ParseAllocationViewModelEDIT(new AllocationViewModel(), allocation, myPractices));

            }

            ExcelWorksheet worksheet = ep.Workbook.Worksheets.Add(wsName);

            var format = new ExcelTextFormat();
            format.Delimiter = ';';
            format.TextQualifier = '"';
            format.DataTypes = new[] { eDataTypes.String };

            worksheet.Cells["A2"].LoadFromText("Practice Name");
            worksheet.Cells["B2"].LoadFromText("Postcode");
            worksheet.Cells["C2"].LoadFromText(GetAttributeDisplayName("Year3B1Allocated"));
            worksheet.Cells["D2"].LoadFromText(GetAttributeDisplayName("Year3B2Allocated"));
            worksheet.Cells["E2"].LoadFromText(GetAttributeDisplayName("Year3B3Allocated"));
            worksheet.Cells["F2"].LoadFromText(GetAttributeDisplayName("Year3B4Allocated"));
            worksheet.Cells["G2"].LoadFromText(GetAttributeDisplayName("Year3B5Allocated"));
            worksheet.Cells["H2"].LoadFromText(GetAttributeDisplayName("Year3B6Allocated"));
            worksheet.Cells["I2"].LoadFromText(GetAttributeDisplayName("Year3B7Allocated"));

            var year3Header = worksheet.Cells["C2:I2"];
            year3Header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#f8cbad");
            year3Header.Style.Fill.BackgroundColor.SetColor(colFromHex);
            year3Header.Style.Font.Bold = true;
            year3Header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            year3Header.Style.TextRotation = 90;

            worksheet.Cells["J2"].LoadFromText(GetAttributeDisplayName("Year4B1Allocated"));
            worksheet.Cells["K2"].LoadFromText(GetAttributeDisplayName("Year4B2Allocated"));
            worksheet.Cells["L2"].LoadFromText(GetAttributeDisplayName("Year4B3Allocated"));
            worksheet.Cells["M2"].LoadFromText(GetAttributeDisplayName("Year4B4Allocated"));
            worksheet.Cells["N2"].LoadFromText(GetAttributeDisplayName("Year4B5Allocated"));
            worksheet.Cells["O2"].LoadFromText(GetAttributeDisplayName("Year4B6Allocated"));
            worksheet.Cells["P2"].LoadFromText(GetAttributeDisplayName("Year4B7Allocated"));

            var year4Header = worksheet.Cells["J2:P2"];
            year4Header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            colFromHex = System.Drawing.ColorTranslator.FromHtml("#00b0f0");
            year4Header.Style.Fill.BackgroundColor.SetColor(colFromHex);
            year4Header.Style.Font.Bold = true;
            year4Header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            year4Header.Style.TextRotation = 90;

            worksheet.Cells["Q2"].LoadFromText(GetAttributeDisplayName("Year5B1Allocated"));
            worksheet.Cells["R2"].LoadFromText(GetAttributeDisplayName("Year5B2Allocated"));
            worksheet.Cells["S2"].LoadFromText(GetAttributeDisplayName("Year5B3Allocated"));
            worksheet.Cells["T2"].LoadFromText(GetAttributeDisplayName("Year5B4Allocated"));
            worksheet.Cells["U2"].LoadFromText(GetAttributeDisplayName("Year5B5Allocated"));
            worksheet.Cells["V2"].LoadFromText(GetAttributeDisplayName("Year5B6Allocated"));


            var year5Header = worksheet.Cells["Q2:V2"];
            year5Header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            colFromHex = System.Drawing.ColorTranslator.FromHtml("#ffe699");
            year5Header.Style.Fill.BackgroundColor.SetColor(colFromHex);
            year5Header.Style.Font.Bold = true;
            year5Header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            year5Header.Style.TextRotation = 90;

            //worksheet.Cells["W2"].LoadFromText(GetAttributeDisplayName("Year5B1Allocated"));
            //worksheet.Cells["X2"].LoadFromText(GetAttributeDisplayName("Year5B2Allocated"));
            //worksheet.Cells["Y2"].LoadFromText(GetAttributeDisplayName("Year5B3Allocated"));
            //worksheet.Cells["Z2"].LoadFromText(GetAttributeDisplayName("Year5B4Allocated"));
            //worksheet.Cells["AA2"].LoadFromText(GetAttributeDisplayName("Year5B5Allocated"));
            //worksheet.Cells["AB2"].LoadFromText(GetAttributeDisplayName("Year5B6Allocated"));

            //var year5Header = worksheet.Cells["W2:AB2"];
            //year5Header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //colFromHex = System.Drawing.ColorTranslator.FromHtml("#c6e0b4");
            //year5Header.Style.Fill.BackgroundColor.SetColor(colFromHex);
            //year5Header.Style.Font.Bold = true;
            //year5Header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //year5Header.Style.TextRotation = 90;

            //worksheet.Cells["AC2"].LoadFromText(GetAttributeDisplayName("ServiceContractReceived"));

            //var servContrHeader = worksheet.Cells["AC2"];
            //servContrHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //colFromHex = System.Drawing.ColorTranslator.FromHtml("#e73c3c");
            //servContrHeader.Style.Fill.BackgroundColor.SetColor(colFromHex);
            //servContrHeader.Style.Font.Bold = true;
            //servContrHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //servContrHeader.Style.TextRotation = 90;

            int rowCounter = 3;

            string myRange = "C" + rowCounter + ":V" + rowCounter;
            var mainCells = worksheet.Cells[myRange];

            //mainCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //mainCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            foreach (var allocation in allocationViewModel)
            {
                myRange = "C" + rowCounter + ":V" + rowCounter;
                mainCells = worksheet.Cells[myRange];
                mainCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[rowCounter, 1].Value = allocation.Surgery;
                worksheet.Cells[rowCounter, 2].Value = allocation.Postcode;
                worksheet.Cells[rowCounter, 3].Value = allocation.Year3B1Allocated;
                worksheet.Cells[rowCounter, 4].Value = allocation.Year3B2Allocated;
                worksheet.Cells[rowCounter, 5].Value = allocation.Year3B3Allocated;
                worksheet.Cells[rowCounter, 6].Value = allocation.Year3B4Allocated;
                worksheet.Cells[rowCounter, 7].Value = allocation.Year3B5Allocated;
                worksheet.Cells[rowCounter, 8].Value = allocation.Year3B6Allocated;
                worksheet.Cells[rowCounter, 9].Value = allocation.Year3B7Allocated;

                worksheet.Cells[rowCounter, 10].Value = allocation.Year4B1Allocated;
                worksheet.Cells[rowCounter, 11].Value = allocation.Year4B2Allocated;
                worksheet.Cells[rowCounter, 12].Value = allocation.Year4B3Allocated;
                worksheet.Cells[rowCounter, 13].Value = allocation.Year4B4Allocated;
                worksheet.Cells[rowCounter, 14].Value = allocation.Year4B5Allocated;
                worksheet.Cells[rowCounter, 15].Value = allocation.Year4B6Allocated;
                worksheet.Cells[rowCounter, 16].Value = allocation.Year4B7Allocated;

                worksheet.Cells[rowCounter, 17].Value = allocation.Year5B1Allocated;
                worksheet.Cells[rowCounter, 18].Value = allocation.Year5B2Allocated;
                worksheet.Cells[rowCounter, 19].Value = allocation.Year5B3Allocated;
                worksheet.Cells[rowCounter, 20].Value = allocation.Year5B4Allocated;
                worksheet.Cells[rowCounter, 21].Value = allocation.Year5B5Allocated;
                worksheet.Cells[rowCounter, 22].Value = allocation.Year5B6Allocated;


                //worksheet.Cells[rowCounter, 23].Value = allocation.Year5B1Allocated;
                //worksheet.Cells[rowCounter, 24].Value = allocation.Year5B2Allocated;
                //worksheet.Cells[rowCounter, 25].Value = allocation.Year5B3Allocated;
                //worksheet.Cells[rowCounter, 26].Value = allocation.Year5B4Allocated;
                //worksheet.Cells[rowCounter, 27].Value = allocation.Year5B5Allocated;
                //worksheet.Cells[rowCounter, 28].Value = allocation.Year5B6Allocated;

                //worksheet.Cells[rowCounter, 29].Value = ShowServiceContract(allocation.ServiceContractReceived);

                rowCounter++;
            }



            return worksheet;
        }

        private ExcelWorksheet createWorksheetRequested(string wsName, ExcelPackage ep)
        {
            var academicYear = AcademicYearDD();

            var myAllocation = _allocationService.GetByAcademicYear(academicYear);
            var myPractices = _practiceService.GetAll();

            List<AllocationViewModel> allocationViewModel = new List<AllocationViewModel>();

            foreach (var allocation in myAllocation)
            {
                allocationViewModel.Add(ParseAllocationViewModelEDIT(new AllocationViewModel(), allocation, myPractices));

            }


            ExcelWorksheet worksheet = ep.Workbook.Worksheets.Add(wsName);

            var format = new ExcelTextFormat();
            format.Delimiter = ';';
            format.TextQualifier = '"';
            format.DataTypes = new[] { eDataTypes.String };

            worksheet.Cells["A2"].LoadFromText("Practice Name");
            worksheet.Cells["B2"].LoadFromText("Postcode");
            worksheet.Cells["C2"].LoadFromText(GetAttributeDisplayName("Year3B1Requested"));
            worksheet.Cells["D2"].LoadFromText(GetAttributeDisplayName("Year3B2Requested"));
            worksheet.Cells["E2"].LoadFromText(GetAttributeDisplayName("Year3B3Requested"));
            worksheet.Cells["F2"].LoadFromText(GetAttributeDisplayName("Year3B4Requested"));
            worksheet.Cells["G2"].LoadFromText(GetAttributeDisplayName("Year3B5Requested"));
            worksheet.Cells["H2"].LoadFromText(GetAttributeDisplayName("Year3B6Requested"));
            worksheet.Cells["I2"].LoadFromText(GetAttributeDisplayName("Year3B7Requested"));

            var year3Header = worksheet.Cells["C2:I2"];
            year3Header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#f8cbad");
            year3Header.Style.Fill.BackgroundColor.SetColor(colFromHex);
            year3Header.Style.Font.Bold = true;
            year3Header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            year3Header.Style.TextRotation = 90;

            worksheet.Cells["J2"].LoadFromText(GetAttributeDisplayName("Year4B1Requested"));
            worksheet.Cells["K2"].LoadFromText(GetAttributeDisplayName("Year4B2Requested"));
            worksheet.Cells["L2"].LoadFromText(GetAttributeDisplayName("Year4B3Requested"));
            worksheet.Cells["M2"].LoadFromText(GetAttributeDisplayName("Year4B4Requested"));
            worksheet.Cells["N2"].LoadFromText(GetAttributeDisplayName("Year4B5Requested"));
            worksheet.Cells["O2"].LoadFromText(GetAttributeDisplayName("Year4B6Requested"));
            worksheet.Cells["P2"].LoadFromText(GetAttributeDisplayName("Year4B7Requested"));

            var year4Header = worksheet.Cells["J2:P2"];
            year4Header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            colFromHex = System.Drawing.ColorTranslator.FromHtml("#00b0f0");
            year4Header.Style.Fill.BackgroundColor.SetColor(colFromHex);
            year4Header.Style.Font.Bold = true;
            year4Header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            year4Header.Style.TextRotation = 90;

            worksheet.Cells["Q2"].LoadFromText(GetAttributeDisplayName("Year5B1Requested"));
            worksheet.Cells["R2"].LoadFromText(GetAttributeDisplayName("Year5B2Requested"));
            worksheet.Cells["S2"].LoadFromText(GetAttributeDisplayName("Year5B3Requested"));
            worksheet.Cells["T2"].LoadFromText(GetAttributeDisplayName("Year5B4Requested"));
            worksheet.Cells["U2"].LoadFromText(GetAttributeDisplayName("Year5B5Requested"));
            worksheet.Cells["V2"].LoadFromText(GetAttributeDisplayName("Year5B6Requested"));


            var year5Header = worksheet.Cells["Q2:V2"];
            year5Header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            colFromHex = System.Drawing.ColorTranslator.FromHtml("#ffe699");
            year5Header.Style.Fill.BackgroundColor.SetColor(colFromHex);
            year5Header.Style.Font.Bold = true;
            year5Header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            year5Header.Style.TextRotation = 90;


            int rowCounter = 3;

            string myRange = "C" + rowCounter + ":V" + rowCounter;
            var mainCells = worksheet.Cells[myRange];


            foreach (var allocation in allocationViewModel)
            {
                myRange = "C" + rowCounter + ":V" + rowCounter;
                mainCells = worksheet.Cells[myRange];
                mainCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[rowCounter, 1].Value = allocation.Surgery;
                worksheet.Cells[rowCounter, 2].Value = allocation.Postcode;
                worksheet.Cells[rowCounter, 3].Value = allocation.Year3B1Requested;
                worksheet.Cells[rowCounter, 4].Value = allocation.Year3B2Requested;
                worksheet.Cells[rowCounter, 5].Value = allocation.Year3B3Requested;
                worksheet.Cells[rowCounter, 6].Value = allocation.Year3B4Requested;
                worksheet.Cells[rowCounter, 7].Value = allocation.Year3B5Requested;
                worksheet.Cells[rowCounter, 8].Value = allocation.Year3B6Requested;
                worksheet.Cells[rowCounter, 9].Value = allocation.Year3B7Requested;

                worksheet.Cells[rowCounter, 10].Value = allocation.Year4B1Requested;
                worksheet.Cells[rowCounter, 11].Value = allocation.Year4B2Requested;
                worksheet.Cells[rowCounter, 12].Value = allocation.Year4B3Requested;
                worksheet.Cells[rowCounter, 13].Value = allocation.Year4B4Requested;
                worksheet.Cells[rowCounter, 14].Value = allocation.Year4B5Requested;
                worksheet.Cells[rowCounter, 15].Value = allocation.Year4B6Requested;
                worksheet.Cells[rowCounter, 16].Value = allocation.Year4B7Requested;

                worksheet.Cells[rowCounter, 17].Value = allocation.Year5B1Requested;
                worksheet.Cells[rowCounter, 18].Value = allocation.Year5B2Requested;
                worksheet.Cells[rowCounter, 19].Value = allocation.Year5B3Requested;
                worksheet.Cells[rowCounter, 20].Value = allocation.Year5B4Requested;
                worksheet.Cells[rowCounter, 21].Value = allocation.Year5B5Requested;
                worksheet.Cells[rowCounter, 22].Value = allocation.Year5B6Requested;

                rowCounter++;
            }


            return worksheet;
        }

        private int ManagePracticeStatusGroupGET(Practices myPractice, PracticesExternal practicesExternal = null)
        {
            int myPSG = 0;

            if (myPractice.Active == 1)
            {
                myPSG = 1;
            }

            if (myPractice.Queried == 1)
            {
                myPSG = 2;
            }

            if (myPractice.Disabled == 1)
            {
                myPSG = 3;
            }

            if (practicesExternal != null)
            {
                if (practicesExternal.Active == 1)
                {
                    myPSG = 1;
                }

                if (practicesExternal.Queried == 1)
                {
                    myPSG = 2;
                }

                if (practicesExternal.Disabled == 1)
                {
                    myPSG = 3;
                }
            }

            return myPSG;
        }

        private Practices ManagePracticeStatusGroupPOST(Practices practice)
        {
            switch (practice.PracticeStatusGroup)
            {
                case 1:
                    practice.Active = 1;
                    practice.Disabled = 0;
                    practice.Queried = 0;
                    break;

                case 2:
                    practice.Active = 0;
                    practice.Disabled = 0;
                    practice.Queried = 1;
                    break;

                case 3:
                    practice.Active = 0;
                    practice.Disabled = 1;
                    practice.Queried = 0;
                    break;
            }

            return practice;
        }

        private PracticesExternal ManagePracticeStatusGroupPOST(PracticesExternal practice)
        {
            switch (practice.PracticeStatusGroup)
            {
                case 1:
                    practice.Active = 1;
                    practice.Disabled = 0;
                    practice.Queried = 0;
                    break;

                case 2:
                    practice.Active = 0;
                    practice.Disabled = 0;
                    practice.Queried = 1;
                    break;

                case 3:
                    practice.Active = 0;
                    practice.Disabled = 1;
                    practice.Queried = 0;
                    break;
            }

            return practice;
        }

        private string ShowPracticeStatus(int PracticeStatusGroup)
        {
            string showStatusGroup = "";

            switch (PracticeStatusGroup)
            {
                case 1:
                    showStatusGroup = "Active";
                    break;

                case 2:
                    showStatusGroup = "Dormant";
                    break;

                case 3:
                    showStatusGroup = "Archived";
                    break;
            }

            return showStatusGroup;
        }

        private string ShowBoolean(bool getState)
        {
            string showBoolean = "Yes";

            if (!getState)
            {
                showBoolean = "No";
            }

            return showBoolean;
        }

        private string ShowServiceContract(int serviceContractStatus)
        {
            string showService = "";

            switch (serviceContractStatus)
            {
                case 0:
                    showService = "No";
                    break;

                case 1:
                    showService = "Yes";
                    break;

                case 2:
                    showService = "n/a";
                    break;
            }

            return showService;
        }

        private string GetAttributeDisplayName(string getProperty)
        {
            PropertyInfo property = typeof(AllocationViewModel).GetProperty(getProperty);

            var atts = property.GetCustomAttributes(
                typeof(DisplayNameAttribute), true);
            if (atts.Length == 0)
                return property.Name;
            return (atts[0] as DisplayNameAttribute).DisplayName;
        }

        private string GetAttributeDisplayNamePractice(string getProperty)
        {
            PropertyInfo property = typeof(Practices).GetProperty(getProperty);

            var atts = property.GetCustomAttributes(
                typeof(DisplayNameAttribute), true);
            if (atts.Length == 0)
                return property.Name;
            return (atts[0] as DisplayNameAttribute).DisplayName;
        }
    }
}