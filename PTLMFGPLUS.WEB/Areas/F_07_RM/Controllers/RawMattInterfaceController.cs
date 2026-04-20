using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.SERVICE.S_07_RM;
using System.Linq;
using System.Threading.Tasks;

namespace PTLMFGPLUS.WEB.Areas.F_07_RM.Controllers
{
    [Area("F_07_RM")]
    public class RawMattInterfaceController(IRawMattInterfaceService _RawMattInterface) : Controller
    {
        public IActionResult InterfaceIndex()
        {
            return View();
        }
        #region API
        [HttpGet]
        public async Task<IActionResult> GetInterfaceInfo(string frmdate,string todate)
        {
            var result = await _RawMattInterface.Get_Goods_Info(frmdate, todate);
            if (result == null)
            {
                return BadRequest("No Data Found");
            }
            return Ok(new
            {
                item1 = result.Item1.ToList(),
                item2 = result.Item2.ToList(),
                item3 = result.Item3.ToList()
            });
        }
        #endregion
    }
}
