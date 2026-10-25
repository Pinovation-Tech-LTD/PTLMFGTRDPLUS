using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.SERVICE.S_20_Service;
using PTLMFGPLUS.WEB.Filters;
using System;
using System.Globalization;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_20_Service.EAllRecProduct;

namespace PTLMFGPLUS.WEB.Areas.F_20_Service.Controllers
{
    [Authorize]
    [PagePermission("F_20_Service", "AllRecProduct", "Index")]
    [Area("F_20_Service")]
    public class AllRecProductController(IAllRecProductService service) : Controller
    {
        [HttpGet, Route("F_20_Service/AllRecProduct")]
        public IActionResult Index() => View("AllRecProductIndex");

        [HttpGet]
        public async Task<IActionResult> GetInitialData(string date)
        {
            try { return Ok(await service.GetInitialData(date)); }
            catch { return BadRequest(Error("Service filters could not be loaded.")); }
        }

        [HttpGet]
        public async Task<IActionResult> GetRecords(string type, string fromDate, string toDate, string centerId, string receiveNo = "")
        {
            if (!TryDate(fromDate, out DateTime from) || !TryDate(toDate, out DateTime to) || from > to) return BadRequest(Error("Enter a valid date range in dd-MMM-yyyy format."));
            try { return Ok(await service.GetRecords(type ?? "All", from, to, centerId ?? "000000000000", receiveNo)); }
            catch { return BadRequest(Error("Service records could not be loaded.")); }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees(string type)
        {
            try { return Ok(await service.GetEmployees(type ?? "AssignEng")); }
            catch { return BadRequest(Error("Assignment options could not be loaded.")); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTransfer([FromBody] UpdateRequest request) => await Update(() => service.UpdateTransfer(request), request);

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignEmployee([FromBody] UpdateRequest request) => await Update(() => service.AssignEmployee(request), request, true);

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateHotline([FromBody] UpdateRequest request) => await Update(() => service.UpdateHotline(request), request, true);

        private async Task<IActionResult> Update(Func<Task<bool>> operation, UpdateRequest request, bool employeeRequired = false)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.centerId) || string.IsNullOrWhiteSpace(request.receiveNo)) return BadRequest(Error("Service center and receive number are required."));
            if (employeeRequired && string.IsNullOrWhiteSpace(request.employeeId)) return BadRequest(Error("Select an assignment option."));
            try { return await operation() ? Ok(true) : BadRequest(Error("The service record was not updated.")); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch { return BadRequest(Error("The service record could not be updated.")); }
        }

        private static bool TryDate(string value, out DateTime result) => DateTime.TryParseExact(value, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        private static object Error(string message) => new { objError = message };
    }
}
