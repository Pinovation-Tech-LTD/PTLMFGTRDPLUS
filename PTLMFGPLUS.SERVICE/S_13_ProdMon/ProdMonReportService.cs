using Microsoft.AspNetCore.Http;
using PTLMFGPLUS.ENTITY.E_07_RM;
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static PTL.PRINTRDLC.Entity.ER_13_ProdMon.BO_Production;
using static PTLMFGPLUS.ENTITY.E_13_ProdMon.EProdBudget;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PTLMFGPLUS.SERVICE.S_13_ProdMon
{
    public interface IProdMonReportService
    {
        Exception GetError();
        public Task<IEnumerable<RptProBatchBudget>> GetBatchBudget(string pbmno,string type);
       
    }
    public class ProdMonReportService(ICommonService _common, IUnitOfWork _unitofwork, IHttpContextAccessor _httpContextAccessor): IProdMonReportService
    {
        public Exception GetError()
        {
            return _common.GetError();
        }
        public async Task<IEnumerable<RptProBatchBudget>> GetBatchBudget(string pbmno, string type)
        {
            string partype = (type == "FG") ? "41%" : "0101001%";
            try
            {
                string comcod = _common.GetComcod();
                ClassProAccessParams parms = new ClassProAccessParams();
                parms.StoredProcedure = "SP_ENTRY_BATCH_BUDGET";
                parms.Calltype = "RPTBATCHBUDGET";
                parms.Comp1 = comcod;
                parms.Desc01 = pbmno;
                parms.Desc02 = "%";
                parms.Desc03 = partype;
                var results = await _unitofwork.SP_Call.ListAsync<RptProBatchBudget>(parms);
                return results;
            }
            catch (Exception)
            {
                GetError();
                return Enumerable.Empty<RptProBatchBudget>();
            }
        }
    }
}
