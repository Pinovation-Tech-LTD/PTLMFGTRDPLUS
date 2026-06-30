using Microsoft.AspNetCore.Http;
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_13_ProdMon.EProdBudget;

namespace PTLMFGPLUS.SERVICE.S_13_ProdMon
{
    public interface IProdBudgetService
    {
        Exception GetError();
        public Task<IEnumerable<ProductBudgetNo>> Get_Prod_Budget_No(string date);
    }
    public class ProdBudgetService(ICommonService _common, IUnitOfWork _unitofwork, IHttpContextAccessor _httpContextAccessor):IProdBudgetService
    {
        public Exception GetError()
        {
            return _common.GetError();
        }
        public async Task<IEnumerable<ProductBudgetNo>> Get_Prod_Budget_No(string date)
        {
            try
            {
                string comcod = _common.GetComcod();
                ClassProAccessParams parms = new ClassProAccessParams();
                parms.StoredProcedure = "SP_ENTRY_BATCH_BUDGET";
                parms.Calltype = "GETPROBUDGETNO";
                parms.Comp1 = comcod;
                parms.Desc01 = date;
                var results = await _unitofwork.SP_Call.ListAsync<ProductBudgetNo>(parms);
                return results;
            }
            catch(Exception)
            {
                GetError();
                return Enumerable.Empty<ProductBudgetNo>();
            }
            
        }
    }
}
