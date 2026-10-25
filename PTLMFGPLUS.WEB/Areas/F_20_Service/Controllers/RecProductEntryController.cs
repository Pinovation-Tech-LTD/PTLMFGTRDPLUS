using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.SERVICE.S_20_Service;
using PTLMFGPLUS.WEB.Filters;
using System;
using System.Globalization;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_20_Service.ERecProductEntry;

namespace PTLMFGPLUS.WEB.Areas.F_20_Service.Controllers
{
    [Authorize]
    [PagePermission("F_20_Service", "RecProductEntry", "Index")]
    [Area("F_20_Service")]
    public class RecProductEntryController(IRecProductEntryService service) : Controller
    {
        [HttpGet, Route("F_20_Service/RecProductEntry")]
        public IActionResult Index() => View("RecProductEntryIndex");

        [HttpGet]
        public async Task<IActionResult> GetInitialData()
        {
            try { return Ok(await service.GetInitialData()); }
            catch { return BadRequest(Error("Receive entry options could not be loaded.")); }
        }

        [HttpGet]
        public async Task<IActionResult> GetReceiveNo(string date, string centerId)
        {
            if (!ValidDate(date) || string.IsNullOrWhiteSpace(centerId)) return BadRequest(Error("A valid date and service center are required."));
            try { return Ok(new { receiveNo = await service.GetReceiveNo(date, centerId) }); }
            catch { return BadRequest(Error("Receive number could not be generated.")); }
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(string imei, bool editing = false)
        {
            if (string.IsNullOrWhiteSpace(imei)) return BadRequest(Error("Enter an IMEI or serial number."));
            try { return Ok(await service.GetProduct(imei.Trim(), editing)); }
            catch (InvalidOperationException ex) { return BadRequest(Error(ex.Message)); }
            catch { return BadRequest(Error("Product information could not be loaded.")); }
        }

        [HttpGet]
        public async Task<IActionResult> GetEdit(string receiveNo, string centerId, string imei)
        {
            if (string.IsNullOrWhiteSpace(receiveNo) || string.IsNullOrWhiteSpace(centerId)) return BadRequest(Error("Receive number and service center are required."));
            try { return Ok(await service.GetEdit(receiveNo.Trim(), centerId.Trim(), imei?.Trim() ?? "")); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(Error(ex.Message)); }
            catch { return BadRequest(Error("Receive information could not be loaded.")); }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([FromBody] SaveRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.centerId) || string.IsNullOrWhiteSpace(request.imei)) return BadRequest(Error("Service center and IMEI are required."));
            if (!ValidDate(request.receiveDate) || !ValidDate(request.probableDeliveryDate) || !ValidDate(request.activationDate) || !ValidDate(request.bookingDate)) return BadRequest(Error("Dates must use dd-MMM-yyyy format."));
            if (string.IsNullOrWhiteSpace(request.customerId) || request.customerId == "000000000000") return BadRequest(Error("Select a customer."));
            if (string.IsNullOrWhiteSpace(request.serviceType)) return BadRequest(Error("Select a service type."));
            try { return await service.Save(request) ? Ok(new { receiveNo = request.receiveNo }) : BadRequest(Error("The receive record was not saved.")); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(Error(ex.Message)); }
            catch { return BadRequest(Error("The receive record could not be saved.")); }
        }

        private static bool ValidDate(string value) => DateTime.TryParseExact(value, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        private static object Error(string message) => new { objError = message };
    }
}
