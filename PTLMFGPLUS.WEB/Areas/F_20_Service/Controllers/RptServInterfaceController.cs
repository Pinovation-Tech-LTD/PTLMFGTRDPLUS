using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.SERVICE.S_20_Service;
using PTLMFGPLUS.WEB.Filters;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace PTLMFGPLUS.WEB.Areas.F_20_Service.Controllers
{
    [Authorize]
    [PagePermission("F_20_Service", "RptServInterface", "Index", "Type=All")]
    [Area("F_20_Service")]
    public class RptServInterfaceController(
        IServiceInterfaceService service) : Controller
    {
        [HttpGet]
        [Route("F_20_Service/RptServInterface")]
        public IActionResult Index()
        {
            return View("RptServInterfaceIndex");
        }

        [HttpGet]
        public async Task<IActionResult> GetInitialData()
        {
            var result = await service.GetInitialData();
            var error = service.GetError();

            if (error != null)
            {
                return BadRequest(new
                {
                    objError = "Service interface filters could not be loaded."
                });
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetInterfaceInfo(
            string fromDate,
            string toDate,
            string centerCode,
            string serviceType,
            string imei = "")
        {
            const string dateFormat = "dd-MMM-yyyy";

            if (!DateTime.TryParseExact(
                    fromDate,
                    dateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime parsedFromDate)
                || !DateTime.TryParseExact(
                    toDate,
                    dateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime parsedToDate))
            {
                return BadRequest(new
                {
                    objError = "Enter valid From and To dates in dd-MMM-yyyy format."
                });
            }

            if (parsedFromDate > parsedToDate)
            {
                return BadRequest(new
                {
                    objError = "From date cannot be later than To date."
                });
            }

            centerCode = string.IsNullOrWhiteSpace(centerCode)
                ? "000000000000"
                : centerCode.Trim();
            serviceType = string.IsNullOrWhiteSpace(serviceType)
                ? "0000000"
                : serviceType.Trim();

            var result = await service.GetInterfaceInfo(
                parsedFromDate,
                parsedToDate,
                centerCode,
                serviceType,
                imei?.Trim() ?? string.Empty);
            var error = service.GetError();

            if (error != null)
            {
                return BadRequest(new
                {
                    objError = "Service interface data could not be loaded."
                });
            }

            return Ok(result);
        }
    }
}
