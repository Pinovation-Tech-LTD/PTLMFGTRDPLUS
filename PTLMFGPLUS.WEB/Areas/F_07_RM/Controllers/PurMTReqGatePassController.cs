using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.SERVICE.S_07_RM;
using System.Linq;
using System.Threading.Tasks;

namespace PTLMFGPLUS.WEB.Areas.F_07_RM.Controllers
{
    [Area("F_07_RM")]
    public class PurMTReqGatePassController(IPurMTReqGatePassService _PurMTReqGatePass) : Controller
    {
        public IActionResult PurMTReqGatePassIndex()
        {
            return View();
        }
        #region API
        [HttpGet]
        public async Task<IActionResult> GetPassNo(string curdate1)
        {
            var result = await _PurMTReqGatePass.Get_Last_PNO(curdate1);
            if (result == null)
            {
                return BadRequest("No data");
            }
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetPassData(string curdate1,string searchtext)
        {
            var result = await _PurMTReqGatePass.Get_Find_Res_List(curdate1, searchtext);
            if(result==null)
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
