using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PTLMFGPLUS.ENTITY;
using PTLMFGPLUS.LIB.ConstantInfo.IConstantInfo;
using PTLMFGPLUS.SERVICE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.EnumHelpers;


namespace PTLMFGPLUS.WEB.Controllers
{
    public class CompanyPagePermissionController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IPageRegistry _pageRegistry;
        public CompanyPagePermissionController(IPageRegistry pageRegistry)
        {
            _pageRegistry = pageRegistry;
        }

        public async Task<IActionResult> UserLoginFrmComp(string module = null)
        {
            List<EUserModule> items = null;
            var modulesJson = HttpContext.Session.GetString("UserModules");
            if (!string.IsNullOrEmpty(modulesJson))
            {
                 items = JsonSerializer.Deserialize<List<EUserModule>>(modulesJson);
            }

            return View(items);
        }

        [HttpGet]
        public async Task<JsonResult> GetModuleData(string moduleid)
        {
            var moduleType = Enum.Parse<ModuleType>(moduleid);
            var data = _pageRegistry.GetByModule(moduleType);

            return Json(new { success = true, data });
        }
    }
}

