using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PTLMFGPLUS.SERVICE;
using System;
using System.Threading.Tasks;

namespace PTLMFGPLUS.WEB.Controllers
{
    public class FacebookController : Controller
    {
        private readonly FacebookService _fb;
        private readonly IConfiguration _config;

        public FacebookController(FacebookService fb, IConfiguration config)
        {
            _fb = fb;           
            _config = config;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/facebook/connect")]
        public IActionResult Connect()
        {
            var url = _fb.GetOAuthUrl();
            return Redirect(url);
        }

        [HttpGet("/facebook/callback")]
        public async Task<IActionResult> Callback(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
                return RedirectToAction("Index", new { error = "Facebook denied access" });

            // Exchange code for user access token
            var userToken = await _fb.ExchangeCodeForTokenAsync(code);
            TempData["UserToken"] = userToken;

            // Get pages
            var pages = await _fb.GetPagesAsync(userToken);
            return View("SelectPage", pages);
        }
        [HttpPost("/facebook/select-page")]
        public async Task<IActionResult> SelectPage(string pageId, string pageAccessToken, string pageName)
        {
            TempData["PageId"] = pageId;
            TempData["PageAccessToken"] = pageAccessToken;
            TempData["PageName"] = pageName;

            var forms = await _fb.GetLeadFormsAsync(pageId, pageAccessToken);
            return View("SelectForm", forms);
        }
        [HttpPost("/facebook/select-form")]
        public async Task<IActionResult> SelectForm(string formId, string formName)
        {
            var pageId = TempData["PageId"]?.ToString();
            var pageAccessToken = TempData["PageAccessToken"]?.ToString();
            var pageName = TempData["PageName"]?.ToString();

            // Keep TempData alive for the view
            TempData.Keep();

            // Fetch existing leads from this form
            var leads = await _fb.GetFormLeadsAsync(formId, pageAccessToken);

            // Pass everything to view
            ViewBag.FormId = formId;
            ViewBag.FormName = formName;
            ViewBag.PageName = pageName;
            ViewBag.PageId = pageId;
            ViewBag.Leads = leads;

            return View("FormLeads");
        }
    }
}
