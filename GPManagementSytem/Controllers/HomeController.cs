using GPManagementSytem.Models;
using GPManagementSytem.Services;
using GPManagementSytem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GPManagementSytem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPracticeService _practiceService;
        private readonly IAllocationService _allocationService;

        public HomeController(IPracticeService practiceService, IAllocationService allocationService)
        {
            _practiceService = practiceService;
            _allocationService = allocationService;
        }

        public ActionResult Index()
        {
            return View();
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
                return RedirectToAction("EditAllocation", new { id = id });
            }

            var myPractice = _practiceService.GetById(id);

            AllocationViewModel allocationViewModel = new AllocationViewModel();

            allocationViewModel.PracticeId = myPractice.Id;
            allocationViewModel.Surgery = myPractice.Surgery;
            allocationViewModel.Postcode = myPractice.Postcode;
            allocationViewModel.Notes = myPractice.Notes;

            return View(allocationViewModel);
        }

        public ActionResult EditAllocation(int id)
        {
            int myTest = id;

            return View();
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