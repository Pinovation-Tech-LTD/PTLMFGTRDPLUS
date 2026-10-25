using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.SERVICE.S_07_Inv;
using PTLMFGPLUS.SERVICE;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_07_Inv.EPBMatIssueSingle;

namespace PTLMFGPLUS.WEB.Areas.F_07_Inv.Controllers
{
    [Authorize]
    [Area("F_07_Inv")]
    public class PBMatIssueSingleController(IPBMatIssueSingleService service, ICommonService common) : Controller
    {
        [HttpGet, Route("F_07_Inv/PBMatIssueSingle")]
        public IActionResult PBMatIssueSingleIndex()
        {
            ViewBag.LockIssueDate = common.GetComcod() is "8305" or "8401";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetStores(string type)
        {
            try { return Ok(await service.GetStores(type)); }
            catch { return BadRequest(Error("Store list could not be loaded.")); }
        }

        [HttpGet]
        public async Task<IActionResult> GetPreviousIssues(string date)
        {
            if (!ValidDate(date)) return BadRequest(new { objError = "A valid issue date is required." });
            try { return Ok(await service.GetPreviousIssues(date)); }
            catch { return BadRequest(Error("Previous issue list could not be loaded.")); }
        }

        [HttpGet]
        public async Task<IActionResult> Open(string type, string date, string storeCode, string issueNo, string dprNo)
        {
            if (!ValidDate(date) || string.IsNullOrWhiteSpace(storeCode)) return BadRequest(new { objError = "Date and store are required." });
            try { return Ok(await service.Open(type, date, storeCode, issueNo, dprNo)); }
            catch { return BadRequest(Error("Material issue information could not be loaded.")); }
        }

        [HttpGet]
        public async Task<IActionResult> GetRequisitionItems(string requisitionNo, string storeCode)
        {
            if (string.IsNullOrWhiteSpace(requisitionNo) || string.IsNullOrWhiteSpace(storeCode)) return BadRequest(new { objError = "Requisition and store are required." });
            try { return Ok(await service.GetRequisitionItems(requisitionNo, storeCode)); }
            catch { return BadRequest(Error("Requisition items could not be loaded.")); }
        }

        [HttpPost]
        public async Task<IActionResult> ReloadMaterials([FromBody] ReloadMaterialsRequest request)
        {
            if (request == null || !ValidDate(request.date) || string.IsNullOrWhiteSpace(request.storeCode) || request.requisitionItems.Count == 0) return BadRequest(new { objError = "Store, date and requisition items are required." });
            try { return Ok(await service.ReloadMaterials(request)); }
            catch { return BadRequest(Error("Material list could not be recalculated.")); }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveIssueRequest request)
        {
            if (request == null || !ValidDate(request.date) || string.IsNullOrWhiteSpace(request.issueNo) || string.IsNullOrWhiteSpace(request.storeCode)) return BadRequest(new { objError = "Date, issue number and store are required." });
            if (request.issueRows.Count == 0) return BadRequest(new { objError = "There are no materials to issue." });
            try { return await service.Save(request) ? Ok(true) : BadRequest(Error("Material issue validation or save failed.")); }
            catch { return BadRequest(Error("Material issue could not be saved.")); }
        }

        [HttpPost]
        public async Task<IActionResult> Approve([FromBody] ApproveRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.issueNo) || string.IsNullOrWhiteSpace(request.storeCode)) return BadRequest(new { objError = "Issue number and store are required." });
            try { return await service.Approve(request.issueNo, request.storeCode) ? Ok(true) : BadRequest(Error("Material issue approval failed.")); }
            catch { return BadRequest(Error("Material issue could not be approved.")); }
        }

        private static object Error(string fallback) => new { objError = fallback };
        private static bool ValidDate(string value) => DateTime.TryParseExact(value, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        public class ApproveRequest { public string issueNo { get; set; } public string storeCode { get; set; } }
    }
}
