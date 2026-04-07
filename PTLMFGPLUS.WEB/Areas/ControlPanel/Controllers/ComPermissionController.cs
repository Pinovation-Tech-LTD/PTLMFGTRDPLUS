using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.SERVICE;
using PTLMFGPLUS.SERVICE.ControlPanel;
using System;
using System.Threading.Tasks;



namespace PTLMFGPLUS.WEB.Areas.ControlPanel.Controllers

{
    [Area("ControlPanel")]

    public class ComPermissionController(Icomperm _comperm, IMenuService _menuService) : Controller
    {
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ComPermissionIndex()
        {
            try
            {
                var items = await _menuService.GetModules();
                ViewBag.ddlmodule = items;
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                TempData["uierror"] = "Unexpected error occurred!";
                return View();
            }
        }
    }
}
