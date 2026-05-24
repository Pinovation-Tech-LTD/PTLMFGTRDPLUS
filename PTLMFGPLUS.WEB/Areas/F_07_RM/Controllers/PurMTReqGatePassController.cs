using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.ObjectModelRemoting;
using PTLMFGPLUS.ENTITY.E_07_RM;
using PTLMFGPLUS.SERVICE.S_07_RM;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_07_RM.EPurMTReq;

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
        public async Task<IActionResult> GetPassNo(string todate)
        {
            var result = await _PurMTReqGatePass.Get_Last_PNO(todate);
            var error = _PurMTReqGatePass.GetError();
            if (result == null || error != null)
            {
                return BadRequest(new { objError = error.Message });
            }
            
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetPassData(string curdate1,string searchtext)
        {
            var result = await _PurMTReqGatePass.Get_Find_Res_List(curdate1, searchtext);
            var error = _PurMTReqGatePass.GetError();
            if (result == null || error != null)
            {
                return BadRequest(new { objError = error.Message });
            }
            return Ok(new
            {
                item1 = result.Item1.ToList(),
                item2 = result.Item2.ToList(),
                item3 = result.Item3.ToList()
            });
        }
        [HttpPost]
        public async Task<IActionResult> SaveButtonClick(string qparam,string mGetpNo, string mmGetpDat,string getpref,string mtrNar,string issueData)
        {
            var issueDataList = JsonSerializer.Deserialize<List<EPurMTReqGatePass.SaveIssueDataList>>(issueData);
            var result = await _PurMTReqGatePass.Post_Update_Pur_Approved(qparam, mGetpNo, mmGetpDat,getpref,mtrNar, issueDataList);
            var error = _PurMTReqGatePass.GetError();
            if (result == null || error != null)
            {
                return BadRequest(new { objError = error.Message });
            }

            return Ok();
        }
        #endregion
    }
}
