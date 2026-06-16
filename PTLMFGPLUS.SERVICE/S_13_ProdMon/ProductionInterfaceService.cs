using Microsoft.AspNetCore.Http;
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;
using static PTLMFGPLUS.ENTITY.E_13_ProdMon.EProductionInterface;
namespace PTLMFGPLUS.SERVICE.S_13_ProdMon
{
    public interface IProductionInterfaceService
    {
        Exception GetError();
        public Task<List<object>> Get_Goods_Info(string frmdate, string todate, string type);

    }
    public class ProductionInterfaceService(ICommonService _common, IUnitOfWork _unitofwork, IHttpContextAccessor _httpContextAccessor) : IProductionInterfaceService
    {
        public Exception GetError()
        {
            return _common.GetError();
        }
        public async Task<List<object>> Get_Goods_Info(string frmdate, string todate,string type)
        {
            try
            {
                string comcod = _common.GetComcod();
                ClassProAccessParams parms = new ClassProAccessParams();
                parms.StoredProcedure = "SP_REPORT_PRODUCTION_INTERFACE";
                parms.Calltype = "RPTPRODUCTIONDASHBOARD";
                parms.Comp1 = comcod;
                parms.Desc01 = frmdate;
                parms.Desc02 = todate;
                parms.Desc03 = "41%";
                var results = await _unitofwork.SP_Call.DataSetAsync(parms);
                var finalresult = new List<object>();
                if (results != null && results.Tables.Count > 0)
                {
                    finalresult.Add(results.Tables[0].DataTableToList<InterfaceProductInfoData>());
                    finalresult.Add(results.Tables[1].DataTableToList<InterfaceProductProIssue>());
                    finalresult.Add(results.Tables[2].DataTableToList<InterfaceProductProdEntry>());
                    finalresult.Add(results.Tables[3].DataTableToList<InterfaceProductQcEntry>());
                    finalresult.Add(results.Tables[4].DataTableToList<InterfaceProductFlorRcv>());
                    finalresult.Add(results.Tables[5].DataTableToList<InterfaceProductprodprocess>());
                    finalresult.Add(results.Tables[6].DataTableToList<InterfaceProductIssueApproval>());
                    finalresult.Add(results.Tables[7].DataTableToList<InterfaceProductInterNo>());
                    finalresult.Add(results.Tables[8].DataTableToList<InterfaceProductReWork>());
                }
                return finalresult;
            }
            catch(Exception)
            {
                GetError();
                return new List<object>();
            }
           
        }
    }
}
