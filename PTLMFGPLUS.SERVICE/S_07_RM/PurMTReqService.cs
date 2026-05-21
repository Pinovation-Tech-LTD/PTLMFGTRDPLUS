using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using PTLMFGPLUS.ENTITY;
using PTLMFGPLUS.ENTITY.E_07_RM;
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_07_RM.EPurMTReq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PTLMFGPLUS.SERVICE.S_07_RM
{
    public interface IPurMTReqService
    {
        Exception GetError();              
        public Task<IEnumerable<MatTrnsNo>> Get_MATTRANS_No(string date);
        public Task<IEnumerable<PreviousOrder>> Get_Previous_Order(string date);
        public Task<IEnumerable<ProjectFromList>> Get_Project_From_List();
        public Task<Tuple<IEnumerable<GetMatTransInfo>,IEnumerable<GetMatTransInfoSingleData>>> Get_Mat_Transfer(string mTRNNo, string date);
        public Task<Tuple<IEnumerable<ProjectResourseList>,IEnumerable<ProjectResourseList1>>> Get_Project_Resource_List(string projectcode, string curdate, string findResDesc, string stockCheck);
        public Task<bool> Post_Save_Data(string previousOrderDataid, string mtrref, string mtreqdat, string fromprj, string toprj, string mtrnar,List<EPurMTReq.SelectedItemListSave>selectedItem);
        public Task<bool> ApprovedMTReq(string mtreqno);


    }
    public class PurMTReqService(ICommonService _common, IUnitOfWork _unitofwork, IHttpContextAccessor _httpContextAccessor) : IPurMTReqService
    {
        public Exception GetError()
        {
            return _common.GetError();
        }       
       
        public async Task<IEnumerable<MatTrnsNo>> Get_MATTRANS_No(string date)
        {
            string comcod = _common.GetComcod();          
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_ENTRY_PURCHASE_05";
            parms.Calltype = "LASTMTRNO";
            parms.Comp1 = comcod;
            parms.Desc01 = date;
            var results = await _unitofwork.SP_Call.ListAsync<MatTrnsNo>(parms);
            return results;
        }
        public async Task<IEnumerable<PreviousOrder>> Get_Previous_Order(string date)
        {
            string comcod = _common.GetComcod();
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_ENTRY_PURCHASE_05";
            parms.Calltype = "GetPrevMTRList";
            parms.Comp1 = comcod;
            parms.Desc01 = date;
            var results = await _unitofwork.SP_Call.ListAsync<PreviousOrder>(parms);
            return results;
        }


        public async Task<IEnumerable<ProjectFromList>> Get_Project_From_List()
        {
            string comcod = _common.GetComcod();
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_ENTRY_PURCHASE_05";
            parms.Calltype = "GetProjectFromList";
            parms.Comp1 = comcod;
            parms.Desc01 = "%%";
            
            var results = await _unitofwork.SP_Call.ListAsync<ProjectFromList>(parms);
            return results;
        }
        public async Task<Tuple<IEnumerable<GetMatTransInfo>,IEnumerable<GetMatTransInfoSingleData>>> Get_Mat_Transfer(string mtreqno, string date)
        {
            string comcod = _common.GetComcod();
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_ENTRY_PURCHASE_05";
            parms.Calltype = "PrevMTRInfo";
            parms.Comp1 = comcod;
            parms.Desc01 = mtreqno;
            parms.Desc02 = date;         
            var matTransferInfo = await _unitofwork.SP_Call.ListAsync<GetMatTransInfo, GetMatTransInfoSingleData>(parms);

            return matTransferInfo;
        }

        public async Task<Tuple<IEnumerable<ProjectResourseList>, IEnumerable<ProjectResourseList1>>> Get_Project_Resource_List(string projectcode, string curdate, string findResDesc, string stockCheck)
        {
            string comcod = _common.GetComcod();
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_ENTRY_PURCHASE_05";
            parms.Calltype = "GetProjResList";
            parms.Comp1 = comcod;
            parms.Desc01 = projectcode;
            parms.Desc02 = curdate;
            parms.Desc03 = findResDesc;
            parms.Desc04 = stockCheck;

            var results = await _unitofwork.SP_Call.ListAsync<ProjectResourseList,ProjectResourseList1>(parms);
            return results;
        }
        public async Task<bool> ApprovedMTReq(string mtreqno)
        {
            string comcod = _common.GetComcod();
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_REPORT_TRANSFER_INTERFACE";
            parms.Calltype = "APPROVE_MATERIAL_TRANSFER_REQUISITION";
            parms.Comp1 = comcod;
            parms.Desc01 = mtreqno;
            parms.Desc02 = _common.GetUserId();
            parms.Desc03 = DateTime.UtcNow.ToString();           

            var results = await _unitofwork.SP_Call.ExecuteAsync(parms);
            return results;
        }
        public async Task<bool> Post_Save_Data(string previousOrderDataid, string mtrref, string mtreqdat, string fromprj, string toprj, string mtrnar, List<EPurMTReq.SelectedItemListSave> selectedItem)
        {
            string newmtreqno;

            if (string.IsNullOrEmpty(previousOrderDataid))
            {
                var newmtreq = await Get_MATTRANS_No(mtreqdat);
                newmtreqno = newmtreq.FirstOrDefault()?.maxmtrno;
            }
            else
            {
                newmtreqno = previousOrderDataid;
            }
            string comcod = _common.GetComcod();
           
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_ENTRY_PURCHASE_05";
            parms.Calltype = "INESERTUPDATEMTREQ";
            parms.Comp1 = comcod;
            parms.Desc01 = "PURMTREQB";
            parms.Desc02 = newmtreqno;
            parms.Desc03 = mtreqdat;
            parms.Desc04 = fromprj;
            parms.Desc05 = toprj;
            parms.Desc06 = mtrref;
            parms.Desc07 = mtrnar;
            parms.Desc08 = _common.GetUserId();
            parms.Desc09 = "";
            parms.Desc10 = "";
            parms.Desc11 = DateTime.UtcNow.ToString();

            var results = await _unitofwork.SP_Call.ExecuteAsync(parms);
            if(results==false)
            {
                return results;
            }
            //details save
            foreach(var item in selectedItem)
            {                
                ClassProAccessParams detailsparms = new ClassProAccessParams();
                detailsparms.StoredProcedure = "SP_ENTRY_PURCHASE_05";
                detailsparms.Calltype = "INESERTUPDATEMTREQ";
                detailsparms.Comp1 = comcod;
                detailsparms.Desc01 = "PURMTREQA";
                detailsparms.Desc02 = newmtreqno;
                detailsparms.Desc03 = item.rsircode;
                detailsparms.Desc04 = item.spcfcod;
                detailsparms.Desc05 = item.qty.ToString();
                detailsparms.Desc06 = item.amt.ToString();
                var resultdetails = await _unitofwork.SP_Call.ExecuteAsync(detailsparms);
                if(!resultdetails)
                {
                    return resultdetails;
                }                
            }
            return true;
        }      

    }
}
