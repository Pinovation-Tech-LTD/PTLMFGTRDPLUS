using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.SERVICE.S_20_Service;
using PTLMFGPLUS.WEB.Filters;
using System;
using System.Globalization;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_20_Service.EDiagProductEntry;

namespace PTLMFGPLUS.WEB.Areas.F_20_Service.Controllers
{
    [Authorize]
    [PagePermission("F_20_Service", "DiagProductEntry", "Index")]
    [Area("F_20_Service")]
    public class DiagProductEntryController(IDiagProductEntryService service) : Controller
    {
        [HttpGet, Route("F_20_Service/DiagProductEntry")]
        public IActionResult Index() => View("DiagProductEntryIndex");

        [HttpGet]
        public async Task<IActionResult> Load(string centerId, string receiveNo, string imei, string date)
        {
            if (string.IsNullOrWhiteSpace(centerId) || string.IsNullOrWhiteSpace(receiveNo)) return BadRequest(Error("Service center and receive number are required."));
            try { return Ok(await service.Load(centerId.Trim(), receiveNo.Trim(), imei?.Trim() ?? "", date ?? "")); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(Error(ex.Message)); }
            catch { return BadRequest(Error("Diagnosis information could not be loaded.")); }
        }

        [HttpGet]
        public async Task<IActionResult> GetSpareOptions(string centerId, string date)
        {
            if (string.IsNullOrWhiteSpace(centerId) || !ValidDate(date)) return BadRequest(Error("A service center and valid date are required."));
            try { return Ok(await service.GetSpareOptions(centerId.Trim(), date)); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch { return BadRequest(Error("Spare-part options could not be loaded.")); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([FromBody] SaveRequest request)
        {
            if (!ValidRequest(request?.centerId, request?.receiveNo) || !ValidDate(request.diagnosisDate) || !ValidDate(request.receivedDate)) return BadRequest(Error("Complete the required record and date fields."));
            return await Execute(() => service.SaveDiagnosis(request), "Diagnosis was not saved.");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve([FromBody] WorkflowRequest request)
        {
            if (!ValidRequest(request?.centerId, request?.receiveNo) || !ValidDate(request.diagnosisDate)) return BadRequest(Error("Complete the required record and date fields."));
            return await Execute(() => service.Approve(request), "Spare parts were not approved.");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> QcPass([FromBody] WorkflowRequest request)
        {
            if (!ValidRequest(request?.centerId, request?.receiveNo) || !ValidDate(request.date) || !ValidDate(request.diagnosisDate)) return BadRequest(Error("Complete the required record and date fields."));
            return await Execute(() => service.QcPass(request), "QC was not completed.");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> QcReject([FromBody] WorkflowRequest request)
        {
            if (!ValidRequest(request?.centerId, request?.receiveNo)) return BadRequest(Error("Service center and receive number are required."));
            return await Execute(() => service.QcReject(request), "The product was not referred back.");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ServiceDone([FromBody] WorkflowRequest request)
        {
            if (!ValidRequest(request?.centerId, request?.receiveNo) || !ValidDate(request.date) || !ValidDate(request.diagnosisDate)) return BadRequest(Error("Complete the required record and date fields."));
            return await Execute(() => service.ServiceDone(request), "Service completion was not saved.");
        }

        private async Task<IActionResult> Execute(Func<Task<bool>> action, string failure)
        {
            try { return await action() ? Ok(new { success = true }) : BadRequest(Error(failure)); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(Error(ex.Message)); }
            catch { return BadRequest(Error(failure)); }
        }

        private static bool ValidRequest(string centerId, string receiveNo) => !string.IsNullOrWhiteSpace(centerId) && !string.IsNullOrWhiteSpace(receiveNo);
        private static bool ValidDate(string value) => DateTime.TryParseExact(value, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        private static object Error(string message) => new { objError = message };
    }
}
