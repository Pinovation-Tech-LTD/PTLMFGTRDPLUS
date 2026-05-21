using Microsoft.AspNetCore.Http;
using PTLMFGPLUS.ENTITY.E_07_RM;
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections;
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
        public Task<ApiResponse<bool>> Post_Update_Pur_Approved(string qparam,string mGetpNo, string mmGetpDat, string getpref, string mtrNar, List<SaveIssueDataList> issueData);
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
        public async Task<ApiResponse<bool>> Post_Update_Pur_Approved(string qparam, string mGetpNo, string mmGetpDat, string getpref, string mtrNar, List<SaveIssueDataList> issueData)
        {
            string comcod = _common.GetComcod();
            if (qparam.ToString().Trim().ToLower() == "entry")
            {  
                issueData = issueData.Where(x => x.getpqty > 0).ToList();
                if (issueData.Count == 0)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Please Input Order Qty"
                    };
                }
                for (int i = 0; i < issueData.Count; i++)
                {
                    string mtReqno = issueData[i].mtreqno.ToString();
                    string mrSircode = issueData[i].rsircode.ToString();
                    string mspcfcod = issueData[i].spcfcod.ToString();
                    string stockqty = issueData[i].stockqty.ToString();
                    string mApprovedQty = issueData[i].approveqty.ToString();
                    string balqty = issueData[i].balqty.ToString();

                    ClassProAccessParams parms = new ClassProAccessParams();
                    parms.StoredProcedure = "SP_ENTRY_PURCHASE_05";
                    parms.Calltype = "BALMTREQQTY";
                    parms.Comp1 = comcod;
                    parms.Desc01 = mtReqno;
                    parms.Desc02 = mrSircode;
                    parms.Desc03 = mspcfcod;
                    var resultsBALMTREQQTY = await _unitofwork.SP_Call.ListAsync<SaveIssueDataList>(parms);

                    ClassProAccessParams parms1 = new ClassProAccessParams();
                    parms1.StoredProcedure = "SP_ENTRY_PURCHASE_05";
                    parms1.Calltype = "PrevMTRInfo";
                    parms1.Comp1 = comcod;
                    parms1.Desc01 = mtReqno;
                    parms1.Desc02 = mrSircode;
                    parms1.Desc03 = mspcfcod;
                    var resultsPrevMTRInfo = await _unitofwork.SP_Call.ListAsync<SaveIssueDataList>(parms1);

                    if (comcod == "8201")
                    {
                        if (Convert.ToDouble(stockqty) < Convert.ToDouble(mApprovedQty))
                        {
                            return new ApiResponse<bool>
                            {
                                Success = false,
                                Message = "There is no balance qty in Requisition"
                            };
                        }
                        if (Convert.ToDouble(balqty) < Convert.ToDouble(mApprovedQty))
                        {
                            return new ApiResponse<bool>
                            {
                                Success = false,
                                Message = "There is no balance qty in Requisition"
                            };
                        }
                    }
                    if (!resultsBALMTREQQTY.Any())
                    {
                        continue;
                    }
                    else if (Convert.ToDouble(resultsBALMTREQQTY.First().balqty) <= 0)
                    {
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Message= "There is no balance qty in Requisition"
                        };
                    }
                }

                ClassProAccessParams parmspurreqqgpb = new ClassProAccessParams();
                parmspurreqqgpb.StoredProcedure = "SP_ENTRY_PURCHASE_05";
                parmspurreqqgpb.Calltype = "INSORUPREQGPASS";
                parmspurreqqgpb.Comp1 = comcod;
                parmspurreqqgpb.Desc01 = "PURREQGPB";
                parmspurreqqgpb.Desc02 = mGetpNo;
                parmspurreqqgpb.Desc03 = mmGetpDat;
                parmspurreqqgpb.Desc04 = getpref;
                parmspurreqqgpb.Desc05 = mtrNar;
                parmspurreqqgpb.Desc06 = _common.GetUserId();
                parmspurreqqgpb.Desc07 = "";
                parmspurreqqgpb.Desc08 = "";
                parmspurreqqgpb.Desc09 = "";
                bool resultsPURREQGPB = await _unitofwork.SP_Call.ExecuteAsync(parmspurreqqgpb);
                if (resultsPURREQGPB == false)
                {
                    return new ApiResponse<bool> { Success = false, Message = "Not Update in Table B" };
                }
                for (int i = 0; i < issueData.Count; i++)
                {
                    string mtReqno = issueData[i].mtreqno.ToString();
                    bool dcon = Convert.ToDateTime(issueData[i].mtrdat) <= Convert.ToDateTime(mmGetpDat);
                    if (!dcon)
                    {
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Message = "Approved Date is equal or greater Requisition Date"
                        };
                    }
                    string mRsircode = issueData[i].rsircode.ToString();
                    string mSpcefcod = issueData[i].spcfcod.ToString();
                    double getpqty = Convert.ToDouble(issueData[i].getpqty);
                    string getpamt = issueData[i].getpamt.ToString();
                    double mtrfqty = Convert.ToDouble(issueData[i].getpqty);

                    if (mtrfqty >= getpqty)
                    {
                        if (getpqty > 0)
                        {
                            ClassProAccessParams parmspurreqqgpa = new ClassProAccessParams();
                            parmspurreqqgpa.StoredProcedure = "SP_ENTRY_PURCHASE_05";
                            parmspurreqqgpa.Calltype = "INSORUPREQGPASS";
                            parmspurreqqgpa.Comp1 = comcod;
                            parmspurreqqgpa.Desc01 = "PURREQGPA";
                            parmspurreqqgpa.Desc02 = mGetpNo;
                            parmspurreqqgpa.Desc03 = mtReqno;
                            parmspurreqqgpa.Desc04 = mRsircode;
                            parmspurreqqgpa.Desc05 = mSpcefcod;
                            parmspurreqqgpa.Desc06 = getpqty.ToString();
                            parmspurreqqgpa.Desc07 = getpamt;
                            bool resultsPURREQGPA = await _unitofwork.SP_Call.ExecuteAsync(parmspurreqqgpa);
                            if (resultsPURREQGPA == false)
                            {
                                return new ApiResponse<bool> { Success = false, Message = "Not Update in Table A" };
                            }
                        }
                        else
                        {
                            return new ApiResponse<bool>
                            {
                                Success = false,
                                Message = "Order Qty Less then or Equal Balance Qty"
                            };
                        }
                    }
                }                
            }
            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Data Updated successfully"
            };
            //if (ConstantInfo.LogStatus == true)
            //{
            //    string eventtype = "Order Process";
            //    string eventdesc = "Update Process";
            //    string eventdesc2 = mGetpNo;
            //    bool IsVoucherSaved = CALogRecord.AddLogRecord(comcod, ((Hashtable)Session["tblLogin"]), eventtype, eventdesc, eventdesc2);
            //}
        }
    }
}
