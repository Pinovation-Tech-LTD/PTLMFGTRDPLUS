using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.ENTITY.E_07_RM;
using PTLMFGPLUS.SERVICE;
using PTLMFGPLUS.SERVICE.S_07_RM;
using System.Linq;
using System.Threading.Tasks;

namespace PTLMFGPLUS.WEB.Areas.F_07_RM.Controllers
{
    [Area("F_07_RM")]
    public class MaterialsTransferController(ICommonService _common,IMaterialsTransferService _materialsTransfer) : Controller
    {   
        public IActionResult MaterialTransferIndex()
        {
            return View();
        }
        #region API
        [HttpGet]
        public async Task<IActionResult> GetLastMTRNumber(string date)
        {
            var item = await _materialsTransfer.Get_Last_Trans_No(date);
            var error = _materialsTransfer.GetError();
            if (item == null || error != null)
            {
                return BadRequest(new { objError = error.Message });
            }
            return Ok(item.ToList());
        }
        [HttpGet]
        public async Task<IActionResult>  GetProjectFromList()
        {
            var item = await _materialsTransfer.Get_Project_From_List();
            var error = _materialsTransfer.GetError();
            if(item==null|| error != null)
            {
                return BadRequest(new { objError = error.Message });
            }
            return Ok(item.ToList());
        }
        [HttpGet]
        public async Task<IActionResult> GetGatePassNo(string CurDate1, string SearchText)
        {
            var result = await _materialsTransfer.Get_MtrReq_Pass_List(CurDate1, SearchText);
            var error = _materialsTransfer.GetError();
            if (result == null || error != null)
            {
                return BadRequest(new { objError = error.Message });
            }
            return Ok(new
            {
                tblGatePInfo = result.Item1.ToList(),
                tblRes = result.Item2.ToList(),
                tblGatePNo = result.Item3.ToList()
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetPrevTransferInfo(string mTRNNo, string CurDate1)
        {
            var result = await _materialsTransfer.Get_Prev_Transfer_Info(mTRNNo, CurDate1);
            var error = _materialsTransfer.GetError();
            if (result == null || error != null)
            {
                return BadRequest(new { objError = error.Message });
            }
            return Ok(new
            {
                tblMatTrns = result.Item1.ToList(),
                tblMatTransSingle = result.Item2.ToList(),
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetProjectResourceList (string ProjectCode, string CurDate, string FindResDesc)
        {
            string balcon = CompBalConMat();
            var result = await _materialsTransfer.Get_Project_Resource_List(ProjectCode, CurDate, FindResDesc, balcon);
            var error = _materialsTransfer.GetError();
            if (result == null || error != null)
            {
                return BadRequest(new { objError = error.Message });
            }
            return Ok(new
            {
                tblPorjectResList = result.Item1.ToList(),
                tblSpcf = result.Item2.ToList()
            });
        }
        public string CompBalConMat()
        {            
            string comcod = _common.GetComcod();
            string conbal = "";
            switch(comcod)
            {
                case "3301":
                case "1301":
                case "3101":
                    conbal = "notcon";
                    break;

                default:
                    conbal = "GetProjResList";
                    break;
            }
            return conbal;
        }
        #endregion
    }
}
