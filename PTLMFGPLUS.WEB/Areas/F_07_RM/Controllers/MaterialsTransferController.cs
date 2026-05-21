using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.ENTITY.E_07_RM;
using PTLMFGPLUS.SERVICE.S_07_RM;
using System.Linq;
using System.Threading.Tasks;

namespace PTLMFGPLUS.WEB.Areas.F_07_RM.Controllers
{
    [Area("F_07_RM")]
    public class MaterialsTransferController(IMaterialsTransferService _materialsTransfer) : Controller
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
            return Ok(item.ToList());
        }
        #endregion
    }
}
