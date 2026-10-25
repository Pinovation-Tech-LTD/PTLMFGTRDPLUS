using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using PTLMFGPLUS.ENTITY.E_13_ProdMon;
using PTLMFGPLUS.SERVICE.S_13_ProdMon;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_13_ProdMon.EProdBudget;

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

        [HttpGet]
        public async Task<IActionResult> GetBudgetResources(string batchno, string date)
        {
            var resources = await _prodBudgetService.Get_Budget_Resources(batchno, date);
            var processes = await _prodBudgetService.Get_Budget_Processes(batchno);
            var error = _prodBudgetService.GetError();
            if (error != null) return BadRequest(new { objError = error.Message });
            return Ok(new { resources, processes });
        }

        [HttpGet]
        public async Task<IActionResult> GetReportProducts(string batchno)
        {
            var result = await _prodBudgetService.Get_Report_Products(batchno);
            var error = _prodBudgetService.GetError();
            return error == null ? Ok(result) : BadRequest(new { objError = error.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetBudgetReport(string batchno, string productCode)
        {
            var result = await _prodBudgetService.Get_Budget_Report(batchno, productCode);
            var error = _prodBudgetService.GetError();
            return error == null ? Ok(result) : BadRequest(new { objError = error.Message });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(string batchno, string productCode)
        {
            if (string.IsNullOrWhiteSpace(batchno) || string.IsNullOrWhiteSpace(productCode)) return BadRequest(new { objError = "Budget and product are required." });
            var result = await _prodBudgetService.Delete_Product(batchno, productCode);
            return result ? Ok(true) : BadRequest(new { objError = _prodBudgetService.GetError()?.Message ?? "Product could not be deleted." });
        }

        [HttpPost]
        public async Task<IActionResult> SaveBudgetResources([FromBody] SaveBudgetResourcesRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.batchNo)) return BadRequest(new { objError = "Budget number is required." });
            var result = await _prodBudgetService.Save_Budget_Resources(request.batchNo, request.resources, request.processes);
            return result ? Ok(true) : BadRequest(new { objError = _prodBudgetService.GetError()?.Message ?? "Material input could not be saved." });
        }

        [HttpPost]
        public async Task<IActionResult> UploadImei(string batchno, IFormFile file)
        {
            if (!HasImeiAccess()) return Forbid();
            if (file == null || file.Length == 0 || string.IsNullOrWhiteSpace(batchno)) return BadRequest(new { objError = "Select an IMEI workbook and budget." });
            var extension = Path.GetExtension(file.FileName);
            if (!extension.Equals(".xlsx", System.StringComparison.OrdinalIgnoreCase) && !extension.Equals(".xlsm", System.StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { objError = "Only .xlsx or .xlsm files are supported." });

            var rows = new List<ImeiUploadRow>();
            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            foreach (var row in workbook.Worksheet(1).RowsUsed().Skip(1))
            {
                if (row.Cell(1).IsEmpty() && row.Cell(2).IsEmpty()) continue;
                rows.Add(new ImeiUploadRow { SLNUM = row.Cell(1).GetString(), IMEI1 = row.Cell(2).GetString(), IMEI2 = row.Cell(3).GetString(), IMEI3 = row.Cell(4).GetString(), IMEI4 = row.Cell(5).GetString() });
            }
            if (rows.Count == 0) return BadRequest(new { objError = "The workbook contains no IMEI rows." });
            var result = await _prodBudgetService.Save_Imei(batchno, rows);
            return result ? Ok(new { count = rows.Count }) : BadRequest(new { objError = _prodBudgetService.GetError()?.Message ?? "IMEI upload failed." });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveImei(string batchno)
        {
            if (!HasImeiAccess()) return Forbid();
            var result = await _prodBudgetService.Remove_Imei(batchno);
            return result ? Ok(true) : BadRequest(new { objError = _prodBudgetService.GetError()?.Message ?? "IMEI list could not be removed." });
        }

        [HttpGet]
        public async Task<IActionResult> GetEngineCodes(string batchno, bool assigned)
        {
            if (!IsCompany8421()) return Forbid();
            return Ok(await _prodBudgetService.Get_Engine_Codes(batchno, assigned));
        }

        [HttpPost]
        public async Task<IActionResult> AssignEngineCodes([FromBody] AssignEngineCodesRequest request)
        {
            if (!IsCompany8421()) return Forbid();
            if (request == null || request.items.Count == 0) return BadRequest(new { objError = "Select at least one chassis." });
            var result = await _prodBudgetService.Assign_Engine_Codes(request.batchNo, request.items);
            return result ? Ok(true) : BadRequest(new { objError = _prodBudgetService.GetError()?.Message ?? "Chassis assignment failed." });
        }
       
        [HttpPost]
        public async Task<IActionResult> FinalUpdateButtonClick([FromBody] SaveProductBudgetRequest request)
        {
            if (request == null || request.items.Count == 0 || request.items.All(x => x.bgdwqty <= 0))
                return BadRequest(new { objError = "At least one budget quantity must be greater than zero." });
            if (string.IsNullOrWhiteSpace(request.batchName) || request.batchName.Contains("XXXX", System.StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { objError = "A complete batch name is required." });

            var result = await _prodBudgetService.Post_Product_Selection_Final_Update(request.type, request.date, request.pbnno,
                request.sDate, request.tDate, request.batchName, request.items, request.log);
            var error = _prodBudgetService.GetError();
            if (result==false)
            {
                return BadRequest(new { objError = error?.Message ?? "Budget could not be saved." });
            }
            return Ok(result);
        }
       

        #endregion

        [HttpGet]
        public IActionResult DownloadImeiFormat()
        {
            if (!HasImeiAccess()) return Forbid();
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Sheet1");
            var headers = new[] { "SLNUM", "IMEI1", "IMEI2", "IMEI3", "IMEI4" };
            for (var i = 0; i < headers.Length; i++) sheet.Cell(1, i + 1).Value = headers[i];
            sheet.Range(1, 1, 1, headers.Length).Style.Font.Bold = true;
            sheet.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "IMEI NUMBERS.xlsx");
        }

        private bool HasImeiAccess()
        {
            var company = User.FindFirst("Comcod")?.Value;
            return company == "8401" || company == "8421";
        }

        private bool IsCompany8421() => User.FindFirst("Comcod")?.Value == "8421";
    }

}
