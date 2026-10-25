using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.ENTITY.ControlPanel;
using PTLMFGPLUS.ENTITY;
using PTLMFGPLUS.SERVICE;
using PTLMFGPLUS.SERVICE.ControlPanel;
using PTLMFGPLUS.WEB.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTLMFGPLUS.WEB.Areas.ControlPanel.Controllers
{
    [Authorize]
    [PagePermission("ControlPanel", "ComPermission", "ComPermissionIndex", "")]
    [Area("ControlPanel")]
    public class ComPermissionController(Icomperm comperm, IMenuService menuService) : Controller
    {
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ComPermissionIndex()
        {
            try
            {
                ViewBag.ddlmodule = await menuService.GetModules();
                return View();
            }
            catch
            {
                TempData["uierror"] = "Company modules could not be loaded.";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissions(string moduleId = "AA", bool showAll = false)
        {
            if (string.IsNullOrWhiteSpace(moduleId) || (moduleId != "AA" && moduleId.Length < 2))
            {
                return BadRequest(new { objError = "Select a valid module." });
            }

            try
            {
                if (moduleId != "AA" && !(await menuService.GetModules()).Any(x => x.moduleid == moduleId))
                {
                    return BadRequest(new { objError = "The selected module is not available for this company." });
                }
                return Ok(showAll
                    ? await comperm.ShowAllData(moduleId)
                    : await comperm.ShowData(moduleId));
            }
            catch
            {
                return BadRequest(new { objError = "Company permissions could not be loaded." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PagePermission(
            "ControlPanel",
            "ComPermission",
            "ComPermissionIndex",
            "",
            PagePermissionAccess.Entry)]
        public async Task<IActionResult> SavePermissions([FromBody] ECompanyPage.SaveCompanyPagePermissionRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.moduleId))
            {
                return BadRequest(new { objError = "Select a module before saving." });
            }

            try
            {
                if (request.moduleId != "AA" && !(await menuService.GetModules()).Any(x => x.moduleid == request.moduleId))
                {
                    return BadRequest(new { objError = "The selected module is not available for this company." });
                }
                bool saved = await comperm.SavePermissions(request);
                return saved
                    ? Ok(new { message = "Company page permissions updated successfully." })
                    : BadRequest(new { objError = "Company permissions could not be updated." });
            }
            catch
            {
                return BadRequest(new { objError = "Company permissions could not be updated." });
            }
        }

    }
}
