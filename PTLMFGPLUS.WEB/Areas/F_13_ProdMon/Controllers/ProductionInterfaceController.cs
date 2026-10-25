using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.SERVICE.S_13_ProdMon;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_13_ProdMon.EProductionInterface;

namespace PTLMFGPLUS.WEB.Areas.F_13_ProdMon.Controllers
{
    [Area("F_13_ProdMon")]
    public class ProductionInterfaceController(IProductionInterfaceService _ProductionInterface) : Controller
    {
        public IActionResult ProductionInterfaceIndex()
        {
            return View();
        }    
    #region API
        [HttpGet]
        public async Task<IActionResult> GetInterfaceInfo(string frmdate, string todate,string type)
        {
            if (!DateTime.TryParse(frmdate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fromDate) ||
                !DateTime.TryParse(todate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var toDate))
            {
                return BadRequest(new { objError = "A valid From and To date are required." });
            }

            if (fromDate > toDate)
            {
                return BadRequest(new { objError = "From date cannot be later than To date." });
            }

            type = string.IsNullOrWhiteSpace(type) ? "FG" : type;
            if (!type.Equals("FG", StringComparison.OrdinalIgnoreCase) &&
                !type.Equals("SemiFG", StringComparison.OrdinalIgnoreCase) &&
                !type.Equals("PreSemiFG", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { objError = "Invalid production type." });
            }

            var result = await _ProductionInterface.Get_Goods_Info(frmdate, todate,type);
            var error = _ProductionInterface.GetError();
            if (result == null || result.Count != 9 || error != null)
            {
                return BadRequest(new { objError = error?.Message ?? "Production interface data could not be loaded." });
            }

            return Ok(new
            {
                item0 = ((List<InterfaceProductInfoData>)result[0]).ToList(),
                item1 = ((List<InterfaceProductProIssue>)result[1]).ToList(),
                item2 = ((List<InterfaceProductProdEntry>)result[2]).ToList(),
                item3 = ((List<InterfaceProductQcEntry>)result[3]).ToList(),
                item4 = ((List<InterfaceProductFlorRcv>)result[4]).ToList(),
                item5 = ((List<InterfaceProductprodprocess>)result[5]).ToList(),
                item6 = ((List<InterfaceProductIssueApproval>)result[6]).ToList(),
                item7 = ((List<InterfaceProductInterNo>)result[7]).ToList(),
                item8 = ((List<InterfaceProductReWork>)result[8]).ToList()
            });
        }
        #endregion
    }

}
