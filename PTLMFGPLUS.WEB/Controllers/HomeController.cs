using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.SERVICE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.EClass_Login;

namespace PTLMFGPLUS.WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILoginService _repLog;
        private readonly LicenseService _licenseService;
        public HomeController(ILoginService repLog, LicenseService licenseService)
        {
            _repLog = repLog;
            _licenseService = licenseService;
        }
        public IActionResult Index()
        {
            GetCompanyInfo();
            return View();
        }

        private void GetCompanyInfo()
        {
            var companies = _licenseService.GetCompanies()
          .Where(c => c.IsActive && c.ExpiryDate >= DateTime.Today)
          .ToList();

            ViewBag.Companies = companies;
        }

        [HttpPost]
        public async Task<IActionResult> Index(Login_VM obj)
        {
            if (obj == null)
            {
                GetCompanyInfo();
                return View();
            }
            bool validuser=await IsValidUser(obj.comcod, obj.username, obj.userpass);
            return RedirectToAction("DashboardIndex", "Home");
        }

        private async Task<bool> IsValidUser(string comcod, string username, string password)
        {
            try
            {
                var items = await _repLog.GetValidUser(comcod, username, password);
                if(items == null)
                {
                    return false;
                }
                var item1 = items.Item1;
                if(item1==null || item1.Count() == 0)
                {
                    return false;
                }
                var item = item1?.FirstOrDefault();
                var claims = new List<Claim>
                {
                    new Claim("Comcod", comcod),
                    new Claim("UserId", item.usrid),
                    new Claim("EmpId", item.empid),
                    new Claim(ClaimTypes.Name, item.usrname),                    
                    new Claim(ClaimTypes.Role, item.userrole),                  
                    new Claim("Dept", item.deptcode),
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                var item2 = items.Item2;
                return true;
            }
            catch (Exception ex) 
            {
                return false;
            }
        }



        public IActionResult DashboardIndex()
        {
            return View();
        }


    }
}
