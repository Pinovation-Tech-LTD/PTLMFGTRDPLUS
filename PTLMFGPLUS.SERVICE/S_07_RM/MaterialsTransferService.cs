using Microsoft.AspNetCore.Http;
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_07_RM.EMaterialsTranser;


namespace PTLMFGPLUS.SERVICE.S_07_RM
{
    public interface IMaterialsTransferService
    {
        Exception GetError();
        public Task<IEnumerable<LastTransNo>> Get_Last_Trans_No(string date);
        public Task<IEnumerable<ProjectFromList>> Get_Project_From_List();
        public Task<IEnumerable<MtrReqPassList>> Get_MtrReq_Pass_List(string curdate1, string SearchText);

    }
    public class MaterialsTransferService(ICommonService _common, IUnitOfWork _unitofwork, IHttpContextAccessor _httpContextAccessor): IMaterialsTransferService
    {
        string comcod = _common.GetComcod();
        public Exception GetError()
        {
            return _common.GetError();
        }
         public async Task<IEnumerable<LastTransNo>> Get_Last_Trans_No(string date)
        {           
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_ENTRY_PURCHASE_03";
            parms.Calltype = "LastTransferNo";
            parms.Comp1 = comcod;
            parms.Desc01 = date;
            var results = await _unitofwork.SP_Call.ListAsync<LastTransNo>(parms);
            return results;
        }
        public async Task<IEnumerable<ProjectFromList>> Get_Project_From_List()
        {
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_ENTRY_PURCHASE_03";
            parms.Calltype = "GetProjectFromList";
            parms.Comp1 = comcod;
            parms.Desc01 = "%%";
            var result = await _unitofwork.SP_Call.ListAsync<ProjectFromList>(parms);
            return result;

        }
        
        public async Task<IEnumerable<MtrReqPassList>> Get_MtrReq_Pass_List(string curdate1, string SearchText)
        {
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_ENTRY_PURCHASE_03";
            parms.Calltype = "GETMTREQGPASSLIST";
            parms.Comp1 = comcod;
            parms.Desc01 = curdate1;
            parms.Desc01 = SearchText;
            var result = await _unitofwork.SP_Call.ListAsync<MtrReqPassList>(parms);
            return result;
        }
        
    }
}
