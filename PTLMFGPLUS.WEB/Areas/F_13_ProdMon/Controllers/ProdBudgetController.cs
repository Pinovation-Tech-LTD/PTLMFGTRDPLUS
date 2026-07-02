using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.ENTITY.E_13_ProdMon;
using PTLMFGPLUS.SERVICE.S_13_ProdMon;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_13_ProdMon.EProdBudget;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
       
        [HttpGet]
        public async Task<IActionResult> GetShowProductList(string type,string batchno,string date)
        {
            var result = await _prodBudgetService.Get_Show_Product_List(type,batchno,date);
            var error = _prodBudgetService.GetError();
            if (result == null || error != null)
            {
                return BadRequest(new { objError = error?.Message });
            }
            return Ok(new
            {
                item1 = result.Item1.ToList(),
                item2 = result.Item2.ToList()
            });
        }
       
        [HttpPost]
        public async Task<IActionResult> FinalUpdateButtonClick(string type,string date,string pbnno,string sDate,string tDate,string BatchNameText, 
            string selectedItem,string dtuser)
        {
            var items = JsonSerializer.Deserialize<List<SelectedItemListProductionBudget>>(selectedItem);
            var items1 = JsonSerializer.Deserialize<List<ShowProductBudgetLogEntity>>(dtuser);
            var result = await _prodBudgetService.Post_Product_Selection_Final_Update(type, date, pbnno, sDate, tDate, BatchNameText, items,items1);
            var error = _prodBudgetService.GetError();
            if (result==false)
            {
                return BadRequest(new { objError = error?.Message });
            }
            return Ok(result);
        }
       

        #endregion
    }

}
