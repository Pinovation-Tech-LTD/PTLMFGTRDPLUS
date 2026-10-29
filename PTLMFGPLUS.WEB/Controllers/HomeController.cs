using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.SERVICE;
using PTLMFGPLUS.WEB.Models;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(ILoginService repLog, LicenseService licenseService, IWebHostEnvironment webHostEnvironment)
        {
            _repLog = repLog;
            _licenseService = licenseService;
            _webHostEnvironment = webHostEnvironment;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(nameof(DashboardIndex));
            }

            GetCompanyInfo();
            return View(new Login_VM());
        }

        private void GetCompanyInfo()
        {
            var companies = _licenseService.GetCompanies()
          .Where(c => c.IsActive && c.ExpiryDate >= DateTime.Today)
          .ToList();

            ViewBag.Companies = companies;
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Login_VM obj)
        {
            GetCompanyInfo();
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            var company = GetLicensedCompanies()
                .FirstOrDefault(c => string.Equals(c.comcod, obj.comcod, StringComparison.OrdinalIgnoreCase));
            if (company == null)
            {
                ModelState.AddModelError(nameof(obj.comcod), "The selected company is not licensed or the license has expired.");
                return View(obj);
            }

            bool validuser = await IsValidUser(company, obj.username.Trim(), obj.userpass);
            if (!validuser)
            {
                ModelState.AddModelError(string.Empty, "Please enter a correct user name and password.");
                return View(obj);
            }

            return RedirectToAction(nameof(DashboardIndex));
        }

        private List<CompanyInfo> GetLicensedCompanies()
        {
            return _licenseService.GetCompanies()
                .Where(c => c.IsActive && c.ExpiryDate.Date >= DateTime.Today)
                .ToList();
        }

        private async Task<bool> IsValidUser(CompanyInfo company, string username, string password)
        {
            try
            {
                var items = await _repLog.GetValidUser(company.comcod, username, password);
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
                if (item == null || string.IsNullOrWhiteSpace(item.usrid))
                {
                    return false;
                }

                var databaseData = await _repLog.GetLoginDatabaseData(company.comcod, username, password);
                var databaseUser = databaseData.User ?? item;
                if (string.IsNullOrWhiteSpace(databaseUser.usrid))
                {
                    return false;
                }

                var standard = items.Item2?.FirstOrDefault();
                string clientAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
                
                var fullPath = Path.GetFullPath(_webHostEnvironment.WebRootPath);

                var loginContext = new LoginSessionContext
                {
                    comcod = company.comcod,
                    envpath = fullPath,

                    comnam = company.comname,
                    comsnam = company.comsname,
                    comadd = company.comadd,
                    comadd1 = company.comadd1,
                    comadd2 = company.comadd2,
                    comadd3 = company.comadd3,
                    comadd4 = company.comadd4,
                    comweb = company.comweb,
                    comphone = company.comphone,
                    comemail = company.comemail,
                    comfax = company.comfax,
                    combranch = company.combranch,
                    compcod = company.compcod,
                    deptcode = databaseUser.deptcode,
                    ldeptcode = string.Empty,
                    deptname = string.Empty,
                    modulenam = "AA",
                    username = databaseUser.usrsname,
                    usrname = databaseUser.usrname,
                    usrdesig = databaseUser.usrdesig,
                    compname = clientAddress,
                    terminal = clientAddress,
                    posteddate = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                    usrid = databaseUser.usrid,
                    session = Random.Shared.Next(111111, 1000000).ToString(),
                    trmid = string.Empty,
                    commod = "1",
                    empid = databaseUser.empid,
                    mcomcod = databaseData.CompanyLink.mcomcod,
                    scomcod = databaseData.CompanyLink.scomcod,
                    circle = databaseUser.circle,
                    area = databaseUser.area,
                    region = databaseUser.region,
                    territory = databaseUser.territory,
                    sroute = databaseUser.sroute,
                    location = databaseUser.location,
                    urole = databaseUser.urole,
                    userrole = databaseUser.userrole,
                    compsms = databaseUser.compsms || standard?.compsms == true,
                    mailstus = databaseUser.compmail || standard?.compmail == true,
                    salestype = string.IsNullOrWhiteSpace(databaseUser.salestype) ? standard?.salestype : databaseUser.salestype,
                    hrsdate = databaseData.HrPeriod.hrsdate,
                    hredate = databaseData.HrPeriod.hredate,
                    issostockcheck = databaseUser.issostockcheck,
                    apiurl = databaseUser.apiurl
                };

                HttpContext.Session.Clear();
                HttpContext.Session.SetObjectAsJson("LoginCompany", company);
                HttpContext.Session.SetObjectAsJson("LoginUser", databaseUser);
                HttpContext.Session.SetObjectAsJson("CompanyStandard", standard);
                HttpContext.Session.SetObjectAsJson("LoginContext", loginContext);
                HttpContext.Session.SetObjectAsJson("UserPagePermissions", databaseData.PagePermissions);
                HttpContext.Session.SetObjectAsJson("tblLogin", loginContext);
                HttpContext.Session.SetObjectAsJson("userallinfo", databaseData.ResultSets.ElementAtOrDefault(0) ?? new());
                HttpContext.Session.SetObjectAsJson("tblusrlog", databaseData.ResultSets);
                HttpContext.Session.SetObjectAsJson("tbllog", GetLicensedCompanies());
                HttpContext.Session.SetObjectAsJson("tblHstMenu", Array.Empty<object>());
                HttpContext.Session.SetObjectAsJson("tbllog1", new[]
                {
                    new
                    {
                        company.comcod,
                        comnam = company.comname,
                        comsnam = company.comsname,
                        company.comadd1,
                        company.comadd,
                        usrsname = databaseUser.usrsname,
                        loginContext.session
                    }
                });

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, databaseUser.usrid),
                    new Claim(ClaimTypes.Name, databaseUser.usrname ?? databaseUser.usrsname ?? username),
                    new Claim(ClaimTypes.Role, databaseUser.userrole ?? string.Empty),
                    new Claim("Comcod", company.comcod),
                    new Claim("CompanyName", company.comname ?? string.Empty),
                    new Claim("CompanyShortName", company.comsname ?? string.Empty),
                    new Claim("UserId", databaseUser.usrid),
                    new Claim("UserShortName", databaseUser.usrsname ?? string.Empty),
                    new Claim("EmpId", databaseUser.empid ?? string.Empty),
                    new Claim("Dept", databaseUser.deptcode ?? string.Empty),
                    new Claim("UserRole", databaseUser.userrole ?? string.Empty),
                    new Claim("URole", databaseUser.urole ?? string.Empty),
                    new Claim("Module", loginContext.modulenam),
                    new Claim("SessionId", loginContext.session),
                    new Claim("MainCompanyCode", loginContext.mcomcod ?? string.Empty),
                    new Claim("SubCompanyCode", loginContext.scomcod ?? string.Empty)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return true;
            }
            catch (Exception)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
                return false;
            }
        }



        [Authorize]
        public IActionResult DashboardIndex()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied(string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statusCode = 500, string traceId = "")
        {
            Response.StatusCode = statusCode;
            return View(new ErrorViewModel
            {
                StatusCode = statusCode,
                Message = "The request could not be completed. Please try again or contact support.",
                TraceId = traceId
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index));
        }


    }
}
