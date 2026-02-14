using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.SERVICE;
using System;
using System.IO;
using System.Linq;
using static PTLMFGPLUS.ENTITY.EClass_Login;

namespace PTLMFGPLUS.WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILoginService repLog;
        private readonly LicenseService _licenseService;
        public HomeController(ILoginService _repLog, LicenseService licenseService)
        {
            repLog = _repLog;
            _licenseService = licenseService;           
        }
        public IActionResult Index()
        {
            var companies = _licenseService.GetCompanies()
            .Where(c => c.IsActive && c.ExpiryDate >= DateTime.Today)
            .ToList();

            ViewBag.Companies = companies;
            return View();
        }

        [HttpPost]
        public IActionResult Index(Login_VM obj)
        {
            if (obj == null)
            {
                return View();
            }
            return RedirectToAction("DashboardIndex", "Home");
        }

        public IActionResult DashboardIndex()
        {
            return View();
        }
      

    }
}
