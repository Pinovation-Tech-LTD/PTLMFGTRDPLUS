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
        public Task<IEnumerable<BudgetResource>> Get_Budget_Resources(string batchno, string date);
        public Task<IEnumerable<BudgetProcess>> Get_Budget_Processes(string batchno);
        public Task<IEnumerable<BudgetReportProduct>> Get_Report_Products(string batchno);
        public Task<IEnumerable<BudgetReportRow>> Get_Budget_Report(string batchno, string productCode);
        public Task<bool> Delete_Product(string batchno, string productCode);
        public Task<bool> Save_Budget_Resources(string batchno, List<BudgetResource> resources, List<BudgetProcess> processes);
        public Task<bool> Save_Imei(string batchno, List<ImeiUploadRow> rows);
        public Task<bool> Remove_Imei(string batchno);
        public Task<IEnumerable<EngineCodeRow>> Get_Engine_Codes(string batchno, bool assigned);
        public Task<bool> Assign_Engine_Codes(string batchno, List<EngineCodeRow> items);
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

        public async Task<IEnumerable<BudgetResource>> Get_Budget_Resources(string batchno, string date)
        {
            var parms = Params("SP_ENTRY_BATCH_BUDGET", "SWOWBUDGETEDRES");
            parms.Desc01 = batchno;
            parms.Desc02 = "41%";
            parms.Desc03 = date;
            var results = await _unitofwork.SP_Call.ListAsync<BudgetResource>(parms);
            return results;
        }

        public async Task<IEnumerable<BudgetProcess>> Get_Budget_Processes(string batchno)
        {
            var parms = Params("SP_ENTRY_BATCH_BUDGET", "GET_PRPDDUCT_WISE_PROCESS");
            parms.Desc01 = batchno;
            return await _unitofwork.SP_Call.ListAsync<BudgetProcess>(parms);
        }

        public async Task<IEnumerable<BudgetReportProduct>> Get_Report_Products(string batchno)
        {
            var parms = Params("SP_ENTRY_BATCH_BUDGET", "GETPROCODE");
            parms.Desc01 = "";
            parms.Desc02 = batchno;
            return await _unitofwork.SP_Call.ListAsync<BudgetReportProduct>(parms);
        }

        public async Task<IEnumerable<BudgetReportRow>> Get_Budget_Report(string batchno, string productCode)
        {
            var parms = Params("SP_ENTRY_BATCH_BUDGET", "RPTBATCHBUDGET");
            parms.Desc01 = batchno;
            parms.Desc02 = string.IsNullOrWhiteSpace(productCode) || productCode == "000000000000" ? "%" : productCode + "%";
            parms.Desc03 = "41%";
            return await _unitofwork.SP_Call.ListAsync<BudgetReportRow>(parms);
        }

        public async Task<bool> Delete_Product(string batchno, string productCode)
        {
            var parms = Params("SP_ENTRY_BATCH_BUDGET", "DELETEPRO");
            parms.Desc01 = batchno;
            parms.Desc02 = productCode;
            return await _unitofwork.SP_Call.ExecuteAsync(parms);
        }

        public async Task<bool> Save_Budget_Resources(string batchno, List<BudgetResource> resources, List<BudgetProcess> processes)
        {
            foreach (var item in resources)
            {
                var rate = Params("SP_ENTRY_BATCH_BUDGET", "INORUPDATEBGDRESRAT");
                rate.Desc01 = item.spcfcod;
                rate.Desc02 = batchno;
                rate.Desc03 = item.rsircode;
                rate.Desc04 = item.bgdrat.ToString(System.Globalization.CultureInfo.InvariantCulture);
                rate.Desc05 = item.bgdqty.ToString(System.Globalization.CultureInfo.InvariantCulture);
                rate.Desc06 = item.ratio.ToString(System.Globalization.CultureInfo.InvariantCulture);
                if (!await _unitofwork.SP_Call.ExecuteAsync(rate)) return false;
            }

            // Web Forms persists only the processes checked by the user.
            var processTable = UtilityList.ListToDataTable(processes.Where(x => x.active).ToList());
            if (processTable.Columns.Contains("comcod")) processTable.Columns.Remove("comcod");
            if (processTable.Columns.Contains("processdesc")) processTable.Columns.Remove("processdesc");
            processTable.TableName = "Table1";
            var semiTable = UtilityList.ListToDataTable(resources.Where(x => x.selected && x.rsircode != null && x.rsircode.StartsWith("0101999")).ToList());
            if (semiTable.Columns.Contains("selected")) semiTable.Columns.Remove("selected");
            semiTable.TableName = "tblsemibgd";
            var processData = new DataSet("dsProductProcess");
            var semiData = new DataSet("ds1");
            processData.Tables.Add(processTable);
            semiData.Tables.Add(semiTable);

            var save = Params("SP_ENTRY_BATCH_BUDGET", "INSERT_OR_UPDATE_BUDGET_PROCESS");
            save.Dxml01 = processData;
            save.Dxml02 = semiData;
            save.Desc01 = batchno;
            save.Desc02 = _common.GetUserId();
            save.Desc03 = _common.GetIPAddress();
            save.Desc04 = "";// _httpContextAccessor.HttpContext?.TraceIdentifier ?? "";
            save.Desc05 = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
            return await _unitofwork.SP_Call.ExecuteAsync(save);
        }

        public async Task<bool> Save_Imei(string batchno, List<ImeiUploadRow> rows)
        {
            var table = UtilityList.ListToDataTable(rows);
            table.TableName = "tblimei";
            var data = new DataSet("ds");
            data.Tables.Add(table);
            var parms = Params("SP_ENTRY_BATCH_BUDGET", "INSERT_OR_UPDATE_IMEI");
            parms.Dxml01 = data;
            parms.Desc01 = batchno;
            return await _unitofwork.SP_Call.ExecuteAsync(parms);
        }

        public async Task<bool> Remove_Imei(string batchno)
        {
            var parms = Params("SP_ENTRY_BATCH_BUDGET", "REMOVE_BUDGET_IMEI");
            parms.Desc01 = batchno;
            return await _unitofwork.SP_Call.ExecuteAsync(parms);
        }

        public async Task<IEnumerable<EngineCodeRow>> Get_Engine_Codes(string batchno, bool assigned)
        {
            var parms = Params("SP_ENTRY_BATCH_BUDGET_02", assigned ? "GET_BUDGETED_ENGINECODE" : "GET_FREE_ENGINECODE");
            if (assigned) parms.Desc01 = batchno;
            return await _unitofwork.SP_Call.ListAsync<EngineCodeRow>(parms);
        }

        public async Task<bool> Assign_Engine_Codes(string batchno, List<EngineCodeRow> items)
        {
            foreach (var item in items)
            {
                var parms = Params("SP_ENTRY_BATCH_BUDGET_02", "UPDATE_BUDGETED_ENGINECODE");
                parms.Desc01 = batchno;
                parms.Desc02 = item.chassis;
                parms.Desc03 = item.engine;
                parms.Desc04 = item.slnum;
                if (!await _unitofwork.SP_Call.ExecuteAsync(parms)) return false;
            }
            return true;
        }

        private ClassProAccessParams Params(string procedure, string callType)
        {
            return new ClassProAccessParams
            {
                Comp1 = _common.GetComcod(),
                StoredProcedure = procedure,
                Calltype = callType
            };
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
            string Terminal = _common.GetIPAddress();
            string Sessionid = "";//_httpContextAccessor.HttpContext?.TraceIdentifier ?? "";

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
            try
            {
                string comcod = _common.GetComcod();
                ClassProAccessParams parms = new ClassProAccessParams();
                parms.StoredProcedure = "SP_ENTRY_BATCH_BUDGET";
                parms.Calltype = "INORUPDATEBGDWRKLOG";
                parms.Comp1 = comcod;
                parms.Desc01 = pbnno;
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
                    string Bgdqtystring = Bgdqty.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    if (Bgdqty > 0)
                    {
                        ClassProAccessParams parms1 = new ClassProAccessParams();
                        parms1.StoredProcedure = "SP_ENTRY_BATCH_BUDGET";
                        parms1.Calltype = "INORUPDATEBUDGET";
                        parms1.Comp1 = comcod;
                        parms1.Desc01 = "000000000000";
                        parms1.Desc02 = pbnno;
                        parms1.Desc03 = ProdCode;
                        parms1.Desc04 = Bgdqtystring;
                        parms1.Desc05 = date;
                        parms1.Desc06 = BatchNameText;
                        parms1.Desc07 = PostedByid;
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
