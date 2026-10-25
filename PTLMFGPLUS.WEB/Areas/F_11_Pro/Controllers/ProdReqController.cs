using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.ENTITY.E_11_Pro;
using PTLMFGPLUS.SERVICE.S_11_Pro;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_11_Pro.EProdReq;

namespace PTLMFGPLUS.WEB.Areas.F_11_Pro.Controllers
{
    [Authorize]
    [Area("F_11_Pro")]
    public class ProdReqController(IProdReqService prodReqService) : Controller
    {
        [HttpGet]
        [Route("F_11_Pro/ProdReq")]
        [Route("F_11_Pro/ProdReq/ProdReqIndex")]
        public IActionResult ProdReqIndex() => View();

        [HttpGet]
        public async Task<IActionResult> GetInitialData(string type, string genno)
        {
            if (!ValidType(type)) return BadRequest(new { objError = "Invalid requisition type." });
            try
            {
                var budgets = await prodReqService.GetBudgets(type, genno);
                var batches = await prodReqService.GetBatches(type);
                return Ok(new { budgets, batches });
            }
            catch
            {
                return BadRequest(new { objError = prodReqService.GetError()?.Message ?? "Initial requisition data could not be loaded." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPreviousDprs(string date, string budgetNo)
        {
            if (!ValidDate(date) || string.IsNullOrWhiteSpace(budgetNo)) return BadRequest(new { objError = "Date and production budget are required." });
            try { return Ok(await prodReqService.GetPreviousDprs(date, budgetNo)); }
            catch { return BadRequest(new { objError = prodReqService.GetError()?.Message ?? "Previous DPR list could not be loaded." }); }
        }

        [HttpGet]
        public async Task<IActionResult> OpenRequisition(string type, string date, string budgetNo, string dprNo)
        {
            if (!ValidType(type) || !ValidDate(date) || string.IsNullOrWhiteSpace(budgetNo)) return BadRequest(new { objError = "Valid date, type and production budget are required." });
            try
            {
                if (string.IsNullOrWhiteSpace(dprNo)) dprNo = await prodReqService.GetNextDprNo(date);
                if (string.IsNullOrWhiteSpace(dprNo)) return BadRequest(new { objError = "DPR number could not be generated." });
                var data = await prodReqService.GetRequisitionData(type, budgetNo, dprNo, date);
                return Ok(new { dprNo, data.products, data.materials });
            }
            catch { return BadRequest(new { objError = prodReqService.GetError()?.Message ?? "Requisition data could not be loaded." }); }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateMaterials([FromBody] GenerateMaterialsRequest request)
        {
            if (request == null || !ValidType(request.type) || string.IsNullOrWhiteSpace(request.budgetNo) || request.products.Count == 0)
                return BadRequest(new { objError = "Production budget and requisition rows are required." });
            if (request.products.Any(x => x.cresqty > x.bbqty)) return BadRequest(new { objError = "Requisition quantity is larger than budget balance." });
            try { return Ok(await prodReqService.GenerateMaterials(request.type, request.budgetNo, request.products)); }
            catch { return BadRequest(new { objError = prodReqService.GetError()?.Message ?? "Material analysis could not be generated." }); }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveProductionRequisitionRequest request)
        {
            if (request == null || !ValidType(request.type) || !ValidDate(request.date) || string.IsNullOrWhiteSpace(request.dprNo) || string.IsNullOrWhiteSpace(request.budgetNo) || string.IsNullOrWhiteSpace(request.batchCode))
                return BadRequest(new { objError = "Date, DPR number, production budget and batch are required." });
            if (request.products.Count == 0) return BadRequest(new { objError = "There are no requisition rows to save." });
            if (request.products.Any(x => x.cresqty < 0) || request.materials.Any(x => x.cresqty < 0)) return BadRequest(new { objError = "Requisition quantity cannot be negative." });
            if (request.products.Any(x => x.cresqty > x.bbqty)) return BadRequest(new { objError = "Requisition quantity is larger than budget balance." });

            try
            {
                var validBudget = (await prodReqService.GetBudgets(request.type, request.budgetNo)).Any(x => x.pbmno == request.budgetNo);
                if (!validBudget) return BadRequest(new { objError = "The selected production budget is not available for this company." });
                var batches = (await prodReqService.GetBatches(request.type)).ToList();
                if (batches.Count > 0 && !batches.Any(x => x.actcode == request.batchCode)) return BadRequest(new { objError = "The selected batch is not available for this company." });

                var result = await prodReqService.Save(request);
                return result ? Ok(true) : BadRequest(new { objError = prodReqService.GetError()?.Message ?? "Production requisition could not be saved." });
            }
            catch { return BadRequest(new { objError = prodReqService.GetError()?.Message ?? "Production requisition could not be saved." }); }
        }

        private static bool ValidType(string type) => type is "Entry" or "EntrySemi" or "PreEntrySemi";
        private static bool ValidDate(string date) => DateTime.TryParseExact(date, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }
}
