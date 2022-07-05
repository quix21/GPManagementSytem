using GPManagementSytem.Models;
using GPManagementSytem.Services;
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

        public HomeController(IPracticeService practiceService)
        {
            _practiceService = practiceService;
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

        public ActionResult AddAllocation()
        {
            AcademicYearDD();

            return View();
        }

        public void AcademicYearDD()
        {
            List<SelectListItem> myList = new List<SelectListItem>();

            int thisYear = DateTime.Now.Year;

            var getThisYear = DateTime.Now.Year.ToString();
            var getNextYear = (thisYear + 1).ToString();
            var getLastYear = (thisYear - 1).ToString();

            //for (int x=1; x<=3; x++)
            //{
            myList.Add(new SelectListItem { Value = getLastYear, Text = getLastYear });
            myList.Add(new SelectListItem { Value = getThisYear, Text = getThisYear, Selected = true });
            myList.Add(new SelectListItem { Value = getNextYear, Text = getNextYear });

            //}

            ViewBag.AcademicYearDD = myList;
            ViewBag.MyTest = myList;
            ViewData["MyTest2"] = myList;

        }
    }
}