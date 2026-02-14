using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PTLMFGPLUS.SERVICE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.EnumHelpers;

namespace PTLMFGPLUS.WEB.Areas.ControlPanel.Controllers
{
    [Area("ControlPanel")]
    public class ResourcePermController(IPageRegistry _pageReg) : Controller
    {
        public IActionResult CompanyPermIndex()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetModules()
        {
            var modules = new List<object>
            {
                new { id = 0, name = "All Modules" }   // Manual entry
            };
            modules.AddRange(Enum.GetValues(typeof(ModuleType))
                     .Cast<ModuleType>()
                     .Select(e => new
                     {
                         id = (int)e,
                         name = e.GetDisplayName()
                     }));

            return Ok(modules);
        }


        [HttpGet]
        public IActionResult GetAllPages()
        {
            var items = _pageReg.GetAll();
            return Ok(items);
        }
        [HttpGet]
        public IActionResult GetPagesModuleWise(int moduleId)
        {
            
            if (moduleId == 0)
            {
                var allPages = _pageReg.GetAll();
                return Ok(allPages);
            }
            if (!Enum.IsDefined(typeof(ModuleType), moduleId))
                return BadRequest("Invalid module type.");
            var module = (ModuleType)moduleId;
            var items = _pageReg.GetByModule(module);
            return Ok(items);
        }

    }
}
