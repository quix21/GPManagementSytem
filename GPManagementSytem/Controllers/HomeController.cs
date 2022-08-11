using GPManagementSytem.Helper;
using GPManagementSytem.Models;
using GPManagementSytem.Security;
using GPManagementSytem.Services;
using GPManagementSytem.SessionManagement;
using GPManagementSytem.ViewModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace GPManagementSytem.Controllers
{
    //[CheckAuthorisation]
    public class HomeController : BaseController
    {
        private readonly IPracticeService _practiceService;
        private readonly IPracticeExternalService _practiceExternalService;
        private readonly IAllocationService _allocationService;
        private readonly IUserService _userService;

        public HomeController(IPracticeService practiceService, IPracticeExternalService practiceExternalService, IAllocationService allocationService, IUserService userService, ISessionManager sessionManager) : base(sessionManager)
        {
            _practiceService = practiceService;
            _practiceExternalService = practiceExternalService;
            _allocationService = allocationService;
            _userService = userService;
        }

        public ActionResult AddPractice()
        {
            var academicYear = AcademicYearDD();

            return View();
        }

        [HttpPost]
        public ActionResult AddPractice(Practices practice)
        {
            if (ModelState.IsValid)
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
                        practice.Disabled = 1;
                        practice.Queried = 0;
                        break;

                    case 3:
                        practice.Active = 0;
                        practice.Disabled = 0;
                        practice.Queried = 1;
                        break;
                }

                practice.DateCreated = DateTime.Now;
                practice.UpdatedBy = 1;
                _practiceService.AddPractice(practice);

                return RedirectToAction("ManagePractices");
            }
            else
            {
                return View();
            }
        }

        public ActionResult EditPracticeExternal(int id)
        {
            var academicYear = AcademicYearDD();

            var myPractice = _practiceService.GetById(id);

            myPractice.PracticeStatusGroup = ManagePracticeStatusGroupGET(myPractice);

            return View(myPractice);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditPracticeExternal(Practices practice)
        {
            //var myPractice = _practiceService.GetById(practice.Id);

            if (ModelState.IsValid)
            {
                ManagePracticeStatusGroupPOST(practice);

                PracticesExternal practicesExternal = ParsePracticeToExternal(practice);

                practicesExternal.RequestedBy = Convert.ToInt32(Session["UserId"].ToString());
                practicesExternal.DateRequested = DateTime.Now;
                practicesExternal.ChangesApproved = false;

                _practiceExternalService.AddPractice(practicesExternal);

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

        public ActionResult ManagePractices()
        {
            List<Practices> myPractices = _practiceService.GetAll();

            return View(myPractices);
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
                //allocation.UpdatedBy = Convert.ToInt32(Session["UserId"].ToString());

                allocation.UpdatedBy = 1;

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

        private PracticesExternal ParsePracticeToExternal(Practices practice)
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

        public void DownloadAtAGlance()
        {

            CreateWorkbook();
        }


        public void CreateWorkbook()
        {
            List<string> wsNames = new List<string>();
            wsNames.Add("Assigned");

            //Create Excel object

            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();

            createWorksheet(wsNames[0].ToString(), ep);

            string fileName = "AtAGlance-" + DateTime.Now.ToString("MM-dd-yyyy_HH-mm") + ".xlsx";

            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;  filename=" + fileName);
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();

        }

        private ExcelWorksheet createWorksheet(string wsName, ExcelPackage ep)
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
            worksheet.Cells["C2"].LoadFromText(GetAttributeDisplayName("Year2Wk1Allocated"));
            worksheet.Cells["D2"].LoadFromText(GetAttributeDisplayName("Year2Wk2Allocated"));
            worksheet.Cells["E2"].LoadFromText(GetAttributeDisplayName("Year2Wk3Allocated"));
            worksheet.Cells["F2"].LoadFromText(GetAttributeDisplayName("Year2Wk4Allocated"));
            worksheet.Cells["G2"].LoadFromText(GetAttributeDisplayName("Year2Wk5Allocated"));
            worksheet.Cells["H2"].LoadFromText(GetAttributeDisplayName("Year2Wk6Allocated"));

            var year2Header = worksheet.Cells["C2:H2"];
            year2Header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#f8cbad");
            year2Header.Style.Fill.BackgroundColor.SetColor(colFromHex);
            year2Header.Style.Font.Bold = true;
            year2Header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            year2Header.Style.TextRotation = 90;

            worksheet.Cells["I2"].LoadFromText(GetAttributeDisplayName("Year3B1Allocated"));
            worksheet.Cells["J2"].LoadFromText(GetAttributeDisplayName("Year3B2Allocated"));
            worksheet.Cells["K2"].LoadFromText(GetAttributeDisplayName("Year3B3Allocated"));
            worksheet.Cells["L2"].LoadFromText(GetAttributeDisplayName("Year3B4Allocated"));
            worksheet.Cells["M2"].LoadFromText(GetAttributeDisplayName("Year3B5Allocated"));
            worksheet.Cells["N2"].LoadFromText(GetAttributeDisplayName("Year3B6Allocated"));
            worksheet.Cells["O2"].LoadFromText(GetAttributeDisplayName("Year3B7Allocated"));

            var year3Header = worksheet.Cells["I2:O2"];
            year3Header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            colFromHex = System.Drawing.ColorTranslator.FromHtml("#00b0f0");
            year3Header.Style.Fill.BackgroundColor.SetColor(colFromHex);
            year3Header.Style.Font.Bold = true;
            year3Header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            year3Header.Style.TextRotation = 90;

            worksheet.Cells["P2"].LoadFromText(GetAttributeDisplayName("Year4B1Allocated"));
            worksheet.Cells["Q2"].LoadFromText(GetAttributeDisplayName("Year4B2Allocated"));
            worksheet.Cells["R2"].LoadFromText(GetAttributeDisplayName("Year4B3Allocated"));
            worksheet.Cells["S2"].LoadFromText(GetAttributeDisplayName("Year4B4Allocated"));
            worksheet.Cells["T2"].LoadFromText(GetAttributeDisplayName("Year4B5Allocated"));
            worksheet.Cells["U2"].LoadFromText(GetAttributeDisplayName("Year4B6Allocated"));
            worksheet.Cells["V2"].LoadFromText(GetAttributeDisplayName("Year4B7Allocated"));

            var year4Header = worksheet.Cells["P2:V2"];
            year4Header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            colFromHex = System.Drawing.ColorTranslator.FromHtml("#ffe699");
            year4Header.Style.Fill.BackgroundColor.SetColor(colFromHex);
            year4Header.Style.Font.Bold = true;
            year4Header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            year4Header.Style.TextRotation = 90;

            worksheet.Cells["W2"].LoadFromText(GetAttributeDisplayName("Year5B1Allocated"));
            worksheet.Cells["X2"].LoadFromText(GetAttributeDisplayName("Year5B2Allocated"));
            worksheet.Cells["Y2"].LoadFromText(GetAttributeDisplayName("Year5B3Allocated"));
            worksheet.Cells["Z2"].LoadFromText(GetAttributeDisplayName("Year5B4Allocated"));
            worksheet.Cells["AA2"].LoadFromText(GetAttributeDisplayName("Year5B5Allocated"));
            worksheet.Cells["AB2"].LoadFromText(GetAttributeDisplayName("Year5B6Allocated"));

            var year5Header = worksheet.Cells["W2:AB2"];
            year5Header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            colFromHex = System.Drawing.ColorTranslator.FromHtml("#c6e0b4");
            year5Header.Style.Fill.BackgroundColor.SetColor(colFromHex);
            year5Header.Style.Font.Bold = true;
            year5Header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            year5Header.Style.TextRotation = 90;

            worksheet.Cells["AC2"].LoadFromText(GetAttributeDisplayName("ServiceContractReceived"));

            var servContrHeader = worksheet.Cells["AC2"];
            servContrHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
            colFromHex = System.Drawing.ColorTranslator.FromHtml("#e73c3c");
            servContrHeader.Style.Fill.BackgroundColor.SetColor(colFromHex);
            servContrHeader.Style.Font.Bold = true;
            servContrHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            servContrHeader.Style.TextRotation = 90;

            int rowCounter = 3;

            string myRange = "C" + rowCounter + ":AC" + rowCounter;
            var mainCells = worksheet.Cells[myRange];

            //mainCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //mainCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            foreach (var allocation in allocationViewModel)
            {
                myRange = "C" + rowCounter + ":AC" + rowCounter;
                mainCells = worksheet.Cells[myRange];
                mainCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[rowCounter, 1].Value = allocation.Surgery;
                worksheet.Cells[rowCounter, 2].Value = allocation.Postcode;
                worksheet.Cells[rowCounter, 3].Value = allocation.Year2Wk1Allocated;
                worksheet.Cells[rowCounter, 4].Value = allocation.Year2Wk2Allocated;
                worksheet.Cells[rowCounter, 5].Value = allocation.Year2Wk3Allocated;
                worksheet.Cells[rowCounter, 6].Value = allocation.Year2Wk4Allocated;
                worksheet.Cells[rowCounter, 7].Value = allocation.Year2Wk5Allocated;
                worksheet.Cells[rowCounter, 8].Value = allocation.Year2Wk6Allocated;

                worksheet.Cells[rowCounter, 9].Value = allocation.Year3B1Allocated;
                worksheet.Cells[rowCounter, 10].Value = allocation.Year3B2Allocated;
                worksheet.Cells[rowCounter, 11].Value = allocation.Year3B3Allocated;
                worksheet.Cells[rowCounter, 12].Value = allocation.Year3B4Allocated;
                worksheet.Cells[rowCounter, 13].Value = allocation.Year3B5Allocated;
                worksheet.Cells[rowCounter, 14].Value = allocation.Year3B6Allocated;
                worksheet.Cells[rowCounter, 15].Value = allocation.Year3B7Allocated;

                worksheet.Cells[rowCounter, 16].Value = allocation.Year4B1Allocated;
                worksheet.Cells[rowCounter, 17].Value = allocation.Year4B2Allocated;
                worksheet.Cells[rowCounter, 18].Value = allocation.Year4B3Allocated;
                worksheet.Cells[rowCounter, 19].Value = allocation.Year4B4Allocated;
                worksheet.Cells[rowCounter, 20].Value = allocation.Year4B5Allocated;
                worksheet.Cells[rowCounter, 21].Value = allocation.Year4B6Allocated;
                worksheet.Cells[rowCounter, 22].Value = allocation.Year4B7Allocated;

                worksheet.Cells[rowCounter, 23].Value = allocation.Year5B1Allocated;
                worksheet.Cells[rowCounter, 24].Value = allocation.Year5B2Allocated;
                worksheet.Cells[rowCounter, 25].Value = allocation.Year5B3Allocated;
                worksheet.Cells[rowCounter, 26].Value = allocation.Year5B4Allocated;
                worksheet.Cells[rowCounter, 27].Value = allocation.Year5B5Allocated;
                worksheet.Cells[rowCounter, 28].Value = allocation.Year5B6Allocated;

                worksheet.Cells[rowCounter, 29].Value = ShowServiceContract(allocation.ServiceContractReceived);

                rowCounter++;
            }



            return worksheet;
        }

        private int ManagePracticeStatusGroupGET(Practices myPractice)
        {
            int myPSG = 0;

            if (myPractice.Active == 1)
            {
                myPSG = 1;
            }

            if (myPractice.Disabled == 1)
            {
                myPSG = 2;
            }

            if (myPractice.Queried == 1)
            {
                myPSG = 3;
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
                    practice.Disabled = 1;
                    practice.Queried = 0;
                    break;

                case 3:
                    practice.Active = 0;
                    practice.Disabled = 0;
                    practice.Queried = 1;
                    break;
            }

            return practice;
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

        //private ExcelRange DoCentre(int rowCounter, ExcelWorksheet worksheet)
        //{
        //    string myRange = "A" + rowCounter + ":R" + rowCounter;

        //    Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#dff4fb");

        //    var myCell = worksheet.Cells[myRange];
        //    myCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //    myCell.Style.Fill.BackgroundColor.SetColor(colFromHex);

        //    return myCell;
        //}

        private string GetAttributeDisplayName(string getProperty)
        {
            PropertyInfo property = typeof(AllocationViewModel).GetProperty(getProperty);

            var atts = property.GetCustomAttributes(
                typeof(DisplayNameAttribute), true);
            if (atts.Length == 0)
                return property.Name;
            return (atts[0] as DisplayNameAttribute).DisplayName;
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
    }
}