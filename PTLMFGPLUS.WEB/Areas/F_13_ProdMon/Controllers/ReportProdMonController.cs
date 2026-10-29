using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using PTLMFGPLUS.SERVICE;
using PTLMFGPLUS.SERVICE.S_07_Inv;
using PTLMFGPLUS.SERVICE.S_13_ProdMon;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PTLMFGPLUS.WEB.Areas.F_13_ProdMon.Controllers
{
    [Area("F_13_ProdMon")]
    public class ReportProdMonController(IProdMonReportService _service, ICommonService _common, IWebHostEnvironment env) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> ProdbudgetPrint(string pbmno, string type)
        {
            try
            {
                string comcod = _common.GetComcod();
                string comnam = "";
                string comadd = _common.GetComAdd();
                string username = _common.GetUserName();
                string compname = "";
                string printdate = System.DateTime.UtcNow.AddHours(6).ToString("dd.MM.yyyy hh:mm:ss tt");

                var rptlist = await _service.GetBatchBudget(pbmno, type);

                string ComLogo = _common.GetComLogoPath();
                var rdlcPath = Path.GetFullPath(Path.Combine(
                env.ContentRootPath,
                "..",
                "PTL.PRINTRDLC",
                "Reports",
                "R_13_ProdMon",
                "RptProdBudget.rdlc"));

                if (!System.IO.File.Exists(rdlcPath))
                    return NotFound("Report file not found: " + rdlcPath);

                using var report = new LocalReport();
                using (var stream = System.IO.File.OpenRead(rdlcPath))
                {
                    report.LoadReportDefinition(stream);
                }
                string footer = $"{compname}  Printed by: {username}  on {printdate}";


                report.EnableExternalImages = true;

                report.SetParameters(new ReportParameter("comnam", comnam));
                report.SetParameters(new ReportParameter("comadd", comadd));
                report.SetParameters(new ReportParameter("ComLogo", ComLogo));
                report.SetParameters(new ReportParameter("pbmno", pbmno));
                report.SetParameters(new ReportParameter("footer", footer));

                report.DataSources.Add(new ReportDataSource("DataSet1", rptlist));

                byte[] pdf = report.Render("PDF");

                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                var sb = new System.Text.StringBuilder();
                for (var e = ex; e != null; e = e.InnerException)
                    sb.AppendLine(e.GetType().Name + ": " + e.Message);
                return Content(sb.ToString());
            }

         }
    }
}
