using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.SERVICE;
using System.Threading.Tasks;

namespace PTLMFGPLUS.WEB.Controllers
{
    [Authorize]
    [Route("StepOfOperation")]
    public class StepOfOperationController(IStepOfOperationService stepOfOperationService) : Controller
    {
        [HttpGet("{moduleId}")]
        public async Task<IActionResult> Index(string moduleId)
        {
            var model = await stepOfOperationService.GetAsync(moduleId);
            if (model == null)
            {
                return Forbid();
            }

            return View(model);
        }
    }
}
