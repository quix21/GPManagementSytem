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
    }
}