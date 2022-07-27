using GPManagementSytem.Models;
using GPManagementSytem.Security;
using GPManagementSytem.Services;
using GPManagementSytem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GPManagementSytem.Controllers
{
    [CheckAuthorisation]
    public class HomeController : Controller
    {
        private readonly IPracticeService _practiceService;
        private readonly IAllocationService _allocationService;

        public HomeController(IPracticeService practiceService, IAllocationService allocationService)
        {
            _practiceService = practiceService;
            _allocationService = allocationService;
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

        public ActionResult EditPractice(int id)
        {
            var academicYear = AcademicYearDD();

            var myPractice = _practiceService.GetById(id);

            if (myPractice.Active == 1)
            {
                myPractice.PracticeStatusGroup = 1;
            }

            if (myPractice.Disabled == 1)
            {
                myPractice.PracticeStatusGroup = 2;
            }

            if (myPractice.Queried == 1)
            {
                myPractice.PracticeStatusGroup = 3;
            }

            return View(myPractice);
        }

        [HttpPost]
        public ActionResult EditPractice(Practices practice)
        {
            var myPractice = _practiceService.GetById(practice.Id);

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

            allocationViewModel = ParseAllocationViewModelEDIT(allocationViewModel, myAllocation);

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

        private AllocationViewModel ParseAllocationViewModelEDIT(AllocationViewModel allocationViewModel, Allocations myAllocation)
        {
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