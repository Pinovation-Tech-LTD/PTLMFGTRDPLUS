using Microsoft.AspNetCore.Mvc;
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
    public class PurMTReqController(IPurMTReqService _purMTReq) : Controller
    {
        public IActionResult PurMTReqIndex()
        {
            return View();
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> GetLastMTRNumber(string date)
        {
            var item = await _purMTReq.Get_MATTRANS_No(date);
            var error = _purMTReq.GetError();
            if (item == null || error != null)
            {
                return BadRequest(new { objError = error.Message });
            }
            return Ok(item.ToList());
        }
        [HttpGet]
        public async Task<IActionResult> GetPreviousOrderList(string date)
        {
            var item = await _purMTReq.Get_Previous_Order(date);
            var error = _purMTReq.GetError();
            if (item == null || error != null)
            {
                return BadRequest(new { objError = error.Message });
            }
            return Ok(item.ToList());
        }
        [HttpGet]
        public async Task<IActionResult> GetProjectFromList()
        {
            var projectlist = await _purMTReq.Get_Project_From_List();
            var error = _purMTReq.GetError();
            if (projectlist == null || error != null)
            {
                return BadRequest(new { objError = error.Message });
            }
            
            return Ok(projectlist.ToList());
        }
        [HttpGet]
        public async Task<IActionResult> GetProjectResourceList(string projectcode, string curdate, string findResDesc, string stockCheck)
        {
            var Resourcelist = await _purMTReq.Get_Project_Resource_List(projectcode, curdate,findResDesc, stockCheck);
            var error = _purMTReq.GetError();
            if (Resourcelist == null || error != null)
            {
                return BadRequest(new { objError = error.Message });
            }
            return Ok(new
            {
                item1 = Resourcelist.Item1.ToList(),
                item2 = Resourcelist.Item2.ToList()
            });
        }
        [HttpPost]
        public async Task<IActionResult> SaveButtonClick(string previousOrderDataid,string mtrref, string mtreqdat, string seletedFrom, string selectedTo, string mtrnar, string selectedItem)
        {
            var items = JsonSerializer.Deserialize<List<SelectedItemListSave>>(selectedItem);            
            var result = await _purMTReq.Post_Save_Data(previousOrderDataid,mtrref, mtreqdat, seletedFrom, selectedTo, mtrnar, items);
            if (result == false)
            {
                return BadRequest("No Update");
            }
            return Ok("Update Successfully");
        }
        [HttpPost]
        public async Task<IActionResult> ApprovedButtonClick(string mtreqno)
        {            
           
            var result = await _purMTReq.ApprovedMTReq(mtreqno);
            if (result == false)
            {
                return BadRequest("No Update");
            }
            return Ok("Update Successfully");
        }

        [HttpGet]
        public async Task<IActionResult> GetMatTransferInfo(string mtrno, string date)
        {
            var projectlist = await _purMTReq.Get_Mat_Transfer(mtrno, date);
            var error = _purMTReq.GetError();
            if (projectlist == null || error != null)
            {
                return BadRequest(new { objError = error.Message });
            }
            return Ok(new
            {
                item1 = projectlist.Item1.ToList(),
                item2 = projectlist.Item2.ToList()
            });
        }
        
        

        #endregion
    }
}
