using Microsoft.AspNetCore.Http;
using PTLMFGPLUS.ENTITY.E_07_RM;
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_07_RM.EPurMTReqGatePass;

namespace PTLMFGPLUS.SERVICE.S_07_RM
{
    public interface IPurMTReqGatePassService
    {
        Exception GetError();
        public Task<IEnumerable<GetLastGetPNO>> Get_Last_PNO(string curdate1);
        public Task<Tuple<IEnumerable<GetMTReqList>, IEnumerable<GetMtreqResource>, IEnumerable<GetMTReqResList>>> Get_Find_Res_List(string curdate1,string searchtext);
    }
    public class PurMTReqGatePassService(ICommonService _common, IUnitOfWork _unitofwork, IHttpContextAccessor _httpContextAccessor) : IPurMTReqGatePassService
    {
        public Exception GetError()
        {
            return _common.GetError();
        }
        public async Task<IEnumerable<GetLastGetPNO>> Get_Last_PNO(string curdate1)
        {
            string comcod = _common.GetComcod();
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_ENTRY_PURCHASE_05";
            parms.Calltype = "GETLASTGETPNO";
            parms.Comp1 = comcod;
            parms.Desc01 = curdate1;
            var results = await _unitofwork.SP_Call.ListAsync<GetLastGetPNO>(parms);
            return results;
        }
        public async Task<Tuple<IEnumerable<GetMTReqList>, IEnumerable<GetMtreqResource>, IEnumerable<GetMTReqResList>>> Get_Find_Res_List(string curdate1, string searchtext)
        {
            string comcod = _common.GetComcod();
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_ENTRY_PURCHASE_05";
            parms.Calltype = "GETMTREQLIST";
            parms.Comp1 = comcod;
            parms.Desc01 = curdate1;
            parms.Desc02 = searchtext;
            var results = await _unitofwork.SP_Call.ListAsync<GetMTReqList,GetMtreqResource,GetMTReqResList>(parms);
            return results;
        }


    }
}
