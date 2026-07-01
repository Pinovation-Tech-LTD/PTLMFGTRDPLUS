using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.ENTITY.E_13_ProdMon;
using PTLMFGPLUS.SERVICE.S_13_ProdMon;
using System.Threading.Tasks;

namespace PTLMFGPLUS.WEB.Areas.F_13_ProdMon.Controllers
{
    [Area("F_13_ProdMon")]
    public class ProdBudgetController(IProdBudgetService _prodBudgetService) : Controller
    {
        
        public IActionResult ProdBudgetIndex()
        {
            return View();
        }
        #region API

        [HttpGet]
        public async Task<IActionResult> GetProdBudgetNo(string date)
        {
            var result = await _prodBudgetService.Get_Prod_Budget_No(date);
            var error = _prodBudgetService.GetError();
            if (result == null || error != null)
            {
                return BadRequest(new { objError = error?.Message });
            }
            return Ok(result);
        }
       
        [HttpGet]
        public async Task<IActionResult> GetPreviousBudgetList(string type)
        {
            var result = await _prodBudgetService.Get_Previous_Budget_List(type);
            var error = _prodBudgetService.GetError();
            if (result == null || error != null)
            {
                return BadRequest(new { objError = error?.Message });
            }
            return Ok(result);
        }
       
        [HttpGet]
        public async Task<IActionResult> GetProductList(string type,string date)
        {
            var result = await _prodBudgetService.Get_Product_List(type,date);
            var error = _prodBudgetService.GetError();
            if (result == null || error != null)
            {
                return BadRequest(new { objError = error?.Message });
            }
            return Ok(result);
        }
       

        #endregion
    }

}
