using PTLMFGPLUS.ENTITY.E_07_Inv;
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_07_Inv.EPBMatIssueSingle;

namespace PTLMFGPLUS.SERVICE.S_07_Inv
{
    public interface IPBMatIssueSingleService
    {
        Exception GetError();
        Task<IEnumerable<Store>> GetStores(string type);
        Task<IEnumerable<PreviousIssue>> GetPreviousIssues(string date);
        Task<IssueOpenResponse> Open(string type, string date, string storeCode, string issueNo, string dprNo);
        Task<IEnumerable<RequisitionItem>> GetRequisitionItems(string requisitionNo, string storeCode);
        Task<IEnumerable<MaterialIssueRow>> ReloadMaterials(ReloadMaterialsRequest request);
        Task<bool> Save(SaveIssueRequest request);
        Task<bool> Approve(string issueNo, string storeCode);
    }

    public class PBMatIssueSingleService(ICommonService common, IUnitOfWork unitOfWork) : IPBMatIssueSingleService
    {
        public Exception GetError() => common.GetError();

        public async Task<IEnumerable<Store>> GetStores(string type)
        {
            var p = Params("SP_ENTRY_PURCHASE_03", "GETSTORENAME");
            p.Desc01 = "%"; p.Desc02 = type == "EntrySemi" ? "Semi" : ""; p.Desc03 = common.GetUserId();
            return await unitOfWork.SP_Call.ListAsync<Store>(p);
        }

        public async Task<IEnumerable<PreviousIssue>> GetPreviousIssues(string date)
        {
            var p = Params("SP_ENTRY_PURCHASE_03", "GETPREVMISSUELIST"); p.Desc01 = date; p.Desc02 = "Normal";
            return await unitOfWork.SP_Call.ListAsync<PreviousIssue>(p);
        }

        public async Task<IssueOpenResponse> Open(string type, string date, string storeCode, string issueNo, string dprNo)
        {
            var response = new IssueOpenResponse();
            var info = Params("SP_ENTRY_PURCHASE_03", "GETPURISUINFO"); info.Desc01 = string.IsNullOrWhiteSpace(issueNo) ? "NEWREQ" : issueNo; info.Desc02 = date;
            var infoSet = await unitOfWork.SP_Call.DataSetAsync(info);
            if (infoSet.Tables.Count > 0) response.issueRows = infoSet.Tables[0].DataTableToList<MaterialIssueRow>();
            if (infoSet.Tables.Count > 1) response.header = infoSet.Tables[1].DataTableToList<IssueHeader>().FirstOrDefault();

            if (string.IsNullOrWhiteSpace(issueNo))
            {
                var next = Params("SP_ENTRY_PURCHASE_03", "GETLASTISUINFO"); next.Desc01 = date;
                response.issueNo = (await unitOfWork.SP_Call.ListAsync<IssueNumber>(next)).FirstOrDefault()?.maxisuno1 ?? "";

                var material = Params("SP_ENTRY_PURCHASE_03", "MATCODELIST");
                material.Desc01 = storeCode; material.Desc02 = date; material.Desc03 = BuildSearch((await GetStores(type)).FirstOrDefault(x => x.actcode == storeCode)?.acttype); material.Desc04 = string.IsNullOrWhiteSpace(dprNo) ? "%" : dprNo;
                var materialSet = await unitOfWork.SP_Call.DataSetAsync(material);
                if (materialSet.Tables.Count > 0) response.availableMaterials = materialSet.Tables[0].DataTableToList<MaterialIssueRow>();
                if (materialSet.Tables.Count > 1) response.requisitions = materialSet.Tables[1].DataTableToList<RequisitionOption>();
                var selectedReq = response.requisitions.FirstOrDefault()?.preqno;
                if (!string.IsNullOrWhiteSpace(selectedReq)) response.requisitionItems = await GetRequisitionItems(selectedReq, storeCode);
            }
            else response.issueNo = response.header?.misuno1 ?? issueNo;
            return response;
        }

        public async Task<IEnumerable<RequisitionItem>> GetRequisitionItems(string requisitionNo, string storeCode)
        {
            var p = Params("SP_ENTRY_MATERIAL_ISSUE", "GET_REQUISITION_WISE_PRODUCT"); p.Desc01 = requisitionNo; p.Desc02 = storeCode;
            return await unitOfWork.SP_Call.ListAsync<RequisitionItem>(p);
        }

        public async Task<IEnumerable<MaterialIssueRow>> ReloadMaterials(ReloadMaterialsRequest request)
        {
            var stores = await GetStores("Entry");
            var data = XmlData("ds2", "tblreqitem", request.requisitionItems);
            var p = Params("SP_ENTRY_MATERIAL_ISSUE", "GET_ITEM_WISE_MAT_LIST"); p.Dxml01 = data;
            p.Desc01 = request.storeCode; p.Desc02 = request.date; p.Desc03 = BuildSearch(stores.FirstOrDefault(x => x.actcode == request.storeCode)?.acttype); p.Desc04 = request.dprNo;
            return await unitOfWork.SP_Call.ListAsync<MaterialIssueRow>(p);
        }

        public async Task<bool> Save(SaveIssueRequest request)
        {
            if (request.issueRows.Count == 0) return false;
            if (!(await GetStores(request.type)).Any(x => x.actcode == request.storeCode)) return false;
            if (!ValidateQuantities(request)) return false;
            var original = request.originalHeader;
            var isEntry = string.Equals(request.type, "Entry", StringComparison.OrdinalIgnoreCase);
            var now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            var postedBy = isEntry || string.IsNullOrWhiteSpace(original?.postedbyid) ? common.GetUserId() : original.postedbyid;
            var terminal = isEntry || string.IsNullOrWhiteSpace(original?.postrmid) ? common.GetIPAddress() : original.postrmid;
            var session = isEntry || string.IsNullOrWhiteSpace(original?.postseson) ? common.GetTerminalId() : original.postseson;
            var sessionPostedDate = common.GetPostedDate();
            var postedDate = isEntry || string.IsNullOrWhiteSpace(original?.entrydat) ? (string.IsNullOrWhiteSpace(sessionPostedDate) ? now : sessionPostedDate) : original.entrydat;
            var editBy = isEntry ? "" : (string.IsNullOrWhiteSpace(original?.editbyid) ? common.GetUserId() : original.editbyid);
            var editDate = isEntry ? "01-Jan-1900" : (string.IsNullOrWhiteSpace(original?.editdat) ? DateTime.Today.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) : original.editdat);

            var header = Params("SP_ENTRY_PURCHASE_03", "UPDATEPURMISSUEINFO"); header.Dxml01 = XmlData("ds2", "tblreqitem", request.requisitionItems);
            header.Desc01 = "MATISSUEB"; header.Desc02 = request.issueNo; header.Desc03 = request.storeCode; header.Desc04 = request.date;
            header.Desc05 = request.remarks ?? ""; header.Desc06 = request.referenceNo ?? ""; header.Desc07 = postedBy; header.Desc08 = terminal;
            header.Desc09 = session; header.Desc10 = postedDate; header.Desc11 = editBy; header.Desc12 = editDate; header.Desc13 = "Normal";
            if (!await unitOfWork.SP_Call.ExecuteAsync(header)) return false;

            foreach (var row in request.issueRows.Where(x => x.isuqty > 0))
            {
                var p = Params("SP_ENTRY_PURCHASE_03", "UPDATEPURMISSUEINFO"); p.Desc01 = "MATISSUEA"; p.Desc02 = request.issueNo;
                p.Desc03 = row.bactcode; p.Desc04 = row.rsircode; p.Desc05 = row.procode; p.Desc06 = row.isuqty.ToString(CultureInfo.InvariantCulture);
                p.Desc07 = row.preqno; p.Desc08 = row.isurate.ToString(CultureInfo.InvariantCulture); p.Desc09 = row.spcfcod; p.Desc10 = request.storeCode;
                if (!await unitOfWork.SP_Call.ExecuteAsync(p)) return false;
            }
            return true;
        }

        public async Task<bool> Approve(string issueNo, string storeCode)
        {
            var p = Params("SP_ENTRY_MATERIAL_ISSUE", "MATERIAL_ISSUE_APPROVE"); p.Desc01 = issueNo; p.Desc02 = storeCode;
            p.Desc03 = common.GetUserId(); p.Desc04 = common.GetIPAddress(); p.Desc05 = common.GetTerminalId();
            p.Desc06 = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            return await unitOfWork.SP_Call.ExecuteAsync(p);
        }

        private bool ValidateQuantities(SaveIssueRequest request)
        {
            if (request.issueRows.Any(x => x.isuqty < 0 || x.isurate < 0 || (!common.GetComcod().StartsWith("87") && !x.rsircode.StartsWith("06") && x.isuqty > x.stockqty))) return false;
            if (request.requisitionItems.Any(x => x.balisu < 0 || x.issuecomp + x.balisu > x.rsqty)) return false;
            if (common.GetComcod() != "8321" && request.requisitionItems.Sum(x => x.balisu + x.issuecomp) == request.requisitionItems.Sum(x => x.rsqty) && request.issueRows.Sum(x => x.isuqty) != request.issueRows.Sum(x => x.balqty)) return false;
            return true;
        }

        private ClassProAccessParams Params(string sp, string call) => new() { Comp1 = common.GetComcod(), StoredProcedure = sp, Calltype = call };
        private static DataSet XmlData<T>(string dataSetName, string tableName, List<T> rows) { var ds = new DataSet(dataSetName); var table = UtilityList.ListToDataTable(rows); table.TableName = tableName; ds.Tables.Add(table); return ds; }
        private static string BuildSearch(string codeType)
        {
            if (string.IsNullOrWhiteSpace(codeType)) return "";
            var parts = new List<string>();
            foreach (var raw in codeType.Split('/', StringSplitOptions.RemoveEmptyEntries))
            {
                var value = raw.Trim(); var dash = value.IndexOf('-');
                parts.Add(dash >= 0 ? $"left(rsircode,'{dash}') between {value.Replace("-", " and ")}" : $"left(rsircode,'{value.Length}') = {value}");
            }
            return parts.Count == 0 ? "" : $"({string.Join(" or ", parts)})";
        }
    }
}
