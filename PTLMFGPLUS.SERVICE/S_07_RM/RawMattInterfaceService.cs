using Microsoft.AspNetCore.Http;
using PTLMFGPLUS.ENTITY.E_07_RM;
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_07_RM.ERawMattInterface;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PTLMFGPLUS.SERVICE.S_07_RM
{
    public interface IRawMattInterfaceService
    {
        Exception GetError();
        public  Task<Tuple<IEnumerable<InterfaceData>, IEnumerable<InterfaceDataWithTrans>, IEnumerable<NumberOfQuantiy>>> Get_Goods_Info(string frmdate,string todate);
    }
    public class RawMattInterfaceService(ICommonService _common, IUnitOfWork _unitofwork, IHttpContextAccessor _httpContextAccessor): IRawMattInterfaceService
    {
        public Exception GetError()
        {
            return _common.GetError();
        }
        public async Task<Tuple<IEnumerable<InterfaceData>, IEnumerable<InterfaceDataWithTrans>, IEnumerable<NumberOfQuantiy>>> Get_Goods_Info(string frmdate,string todate)
        {
           string comcod = _common.GetComcod();
           ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_REPORT_TRANSFER_INTERFACE";
            parms.Calltype = "RPT_MATERIAL_TRANSFER_INTERFACE";
            parms.Comp1 = comcod;
            parms.Desc01 = frmdate;
            parms.Desc02 = todate;
            var results = await _unitofwork.SP_Call.ListAsync<InterfaceData,InterfaceDataWithTrans,NumberOfQuantiy>(parms);
            return results;

        }

    }
}
