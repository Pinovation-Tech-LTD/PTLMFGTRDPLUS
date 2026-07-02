using Microsoft.AspNetCore.Http;
using PTLMFGPLUS.ENTITY.E_07_RM;
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_13_ProdMon.EProdBudget;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PTLMFGPLUS.SERVICE.S_13_ProdMon
{
    public interface IProdBudgetService
    {
        Exception GetError();
        public Task<IEnumerable<ProductBudgetNo>> Get_Prod_Budget_No(string date);
        public Task<IEnumerable<PreviousBudget>> Get_Previous_Budget_List(string type);
        public Task<IEnumerable<ProductList>> Get_Product_List(string type,string date);
        public Task<Tuple< IEnumerable<ShowProductBudgetEntity>,IEnumerable<ShowProductBudgetLogEntity>>> Get_Show_Product_List(string type,string batchno,string date);
        public Task<bool> Post_Product_Selection_Final_Update(string type,string date,string pbnno,string sDate,string tDate, string BatchNameText,
            List<SelectedItemListProductionBudget> selectedItem,List<ShowProductBudgetLogEntity> dtuser);
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
        public async Task<IEnumerable<PreviousBudget>> Get_Previous_Budget_List(string type)
        {
            string partype = (type == "Entry") ? "41%" : (type == "EntrySemi") ? "0101999%" : "0101998%";            
            try
            {
                string comcod = _common.GetComcod();
                ClassProAccessParams parms = new ClassProAccessParams();
                parms.StoredProcedure = "SP_ENTRY_BATCH_BUDGET";
                parms.Calltype = "GETPREPBMNUMBER";
                parms.Comp1 = comcod;
                parms.Desc01 = partype;
                var results = await _unitofwork.SP_Call.ListAsync<PreviousBudget>(parms);
                return results;
            }
            catch(Exception)
            {
                GetError();
                return Enumerable.Empty<PreviousBudget>();
            }
            
        }
        public async Task<IEnumerable<ProductList>> Get_Product_List(string type,string date)
        {
            string filter = (type == "EntrySemi") ? "0101999%" : (type == "EntryPreSemi") ? "0101998%" : "%";            
            string spName = (type == "Entry") ? "SP_ENTRY_BATCH_BUDGET" : "SP_ENTRY_BATCH_BUDGET_02";
            string callType = (type == "Entry") ? "GETMAINPROCODEREQ" : "GETSEMIPRO";
            try
            {
                string comcod = _common.GetComcod();
                ClassProAccessParams parms = new ClassProAccessParams();
                parms.StoredProcedure = spName;
                parms.Calltype = callType;
                parms.Comp1 = comcod;
                parms.Desc01 = filter;
                parms.Desc02 = date;               
                var results = await _unitofwork.SP_Call.ListAsync<ProductList>(parms);
                return results;
            }
            catch(Exception)
            {
                GetError();
                return Enumerable.Empty<ProductList>();
            }
            
        }
        public async Task<Tuple< IEnumerable<ShowProductBudgetEntity>,IEnumerable<ShowProductBudgetLogEntity>>> Get_Show_Product_List(string type, string batchno, string date)
        {
                        
            string spName = (type == "Entry") ? "SP_ENTRY_BATCH_BUDGET" : "SP_ENTRY_BATCH_BUDGET_02";
            string callType = (type == "Entry") ? "SWOWPRODUCT" : "SHOWSEMIPRO";
            try
            {
                string comcod = _common.GetComcod();
                ClassProAccessParams parms = new ClassProAccessParams();
                parms.StoredProcedure = spName;
                parms.Calltype = callType;
                parms.Comp1 = comcod;
                parms.Desc01 = batchno;
                parms.Desc02 = date;               
                var results = await _unitofwork.SP_Call.ListAsync<ShowProductBudgetEntity,ShowProductBudgetLogEntity>(parms);
                return results;
            }
            catch(Exception)
            {
                GetError();
                return new Tuple<IEnumerable<ShowProductBudgetEntity>, IEnumerable<ShowProductBudgetLogEntity>>(
                    Enumerable.Empty<ShowProductBudgetEntity>(),
                    Enumerable.Empty<ShowProductBudgetLogEntity>()
                );
            }
            
        }
        public async Task<bool> Post_Product_Selection_Final_Update(string type, string date, string pbnno, string sDate, string tDate,
            string BatchNameText,List<SelectedItemListProductionBudget> selectedItem,List<ShowProductBudgetLogEntity> dtuser)
        {

            string tblPostedByid = (dtuser.Count == 0) ? "" : dtuser[0].postedbyid;
            string tblPostedtrmid = (dtuser.Count == 0) ? "" : dtuser[0].postrmid;
            string tblPostedSession = (dtuser.Count == 0) ? "" : dtuser[0].postseson;
            string tblPosteddat = (dtuser.Count == 0) ? "01-Jan-1900" : Convert.ToDateTime(dtuser[0].posteddat).ToString("dd-MMM-yyyy hh:mm:ss tt");
            string tblEditByid = (dtuser.Count == 0) ? "" : dtuser[0].editbyid;
            string tblEditDat = (dtuser.Count == 0) ? "01-Jan-1900" : Convert.ToDateTime(dtuser[0].editdat).ToString("dd-MMM-yyyy");

            string userid = _common.GetUserId();
            string Terminal = "::1";
            string Sessionid = "508216";

            string PostedByid = (type == "Entry") ? userid : (tblPostedByid == "") ? userid : tblPostedByid;
            string Posttrmid = (type == "Entry") ? Terminal : (tblPostedtrmid == "") ? Terminal : tblPostedtrmid;
            string PostSession = (type == "Entry") ? Sessionid : (tblPostedSession == "") ? Sessionid : tblPostedSession;
            string Posteddat = (type == "Entry") ? System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt") : (tblPosteddat == "01-Jan-1900") ? System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt") : tblPosteddat;
            string EditByid = (dtuser.Count == 0) ? "" : userid;
            string Editdat = (dtuser.Count == 0) ? "01-Jan-1900" : System.DateTime.Today.ToString("dd-MMM-yyyy");

            double tBgdQty = selectedItem.Sum(x => Convert.ToDouble(x.bgdwqty));
            if(tBgdQty==0)
            {
                return false;
            }
            string newmtpbno;            
                var newmtreq = await Get_Prod_Budget_No(date);
                newmtpbno = newmtreq.FirstOrDefault()?.bpno;
            
            try
            {
                string comcod = _common.GetComcod();
                ClassProAccessParams parms = new ClassProAccessParams();
                parms.StoredProcedure = "SP_ENTRY_BATCH_BUDGET";
                parms.Calltype = "INORUPDATEBGDWRKLOG";
                parms.Comp1 = comcod;
                parms.Desc01 = newmtpbno;
                parms.Desc02 = date;               
                parms.Desc03 = PostedByid;
                parms.Desc04 = Posttrmid;
                parms.Desc05 = PostSession;
                parms.Desc06 = Posteddat;
                parms.Desc07 = EditByid;
                parms.Desc08 = Editdat;
                parms.Desc09 = sDate;
                parms.Desc10 = tDate;
                var results = await _unitofwork.SP_Call.ExecuteAsync(parms);
                if(!results)
                {
                    return false;
                }
                for(int i=0; i < selectedItem.Count; i++)
                {
                    string ProdCode = selectedItem[i].prodcode;
                    double Bgdqty = Convert.ToDouble(selectedItem[i].bgdwqty);
                    string Bgdqtystring = Bgdqty.ToString();
                    if (Bgdqty > 0)
                    {
                        ClassProAccessParams parms1 = new ClassProAccessParams();
                        parms1.StoredProcedure = "SP_ENTRY_BATCH_BUDGET";
                        parms1.Calltype = "INORUPDATEBUDGET";
                        parms1.Comp1 = comcod;
                        parms1.Desc01 = "000000000000";
                        parms1.Desc02 = date;
                        parms1.Desc03 = newmtpbno;
                        parms1.Desc04 = ProdCode;
                        parms1.Desc05 = Bgdqtystring;
                        parms1.Desc06 = date;
                        parms1.Desc07 = BatchNameText;
                        parms1.Desc08 = PostedByid;                        
                        var results1 = await _unitofwork.SP_Call.ExecuteAsync(parms1);
                        if (!results1)
                        {
                            return false;
                        }
                    }
                    
                }
                return true;
            }
            catch(Exception)
            {
                GetError();
                return false;
            }

            
        }

    }
}
