using Microsoft.AspNetCore.Http;
using PTLMFGPLUS.ENTITY.E_11_Pro;
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_11_Pro.EProdReq;

namespace PTLMFGPLUS.SERVICE.S_11_Pro
{
    public interface IProdReqService
    {
        Exception GetError();
        Task<IEnumerable<ProductionBudget>> GetBudgets(string type, string generationNo);
        Task<IEnumerable<BatchCode>> GetBatches(string type);
        Task<IEnumerable<PreviousDpr>> GetPreviousDprs(string date, string budgetNo);
        Task<string> GetNextDprNo(string date);
        Task<RequisitionDataResponse> GetRequisitionData(string type, string budgetNo, string dprNo, string date);
        Task<IEnumerable<RequisitionMaterial>> GenerateMaterials(string type, string budgetNo, List<RequisitionProduct> products);
        Task<bool> Save(SaveProductionRequisitionRequest request);
    }

    public class ProdReqService(ICommonService common, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : IProdReqService
    {
        public Exception GetError() => common.GetError();

        public async Task<IEnumerable<ProductionBudget>> GetBudgets(string type, string generationNo)
        {
            var parms = Params("SP_ENTRY_BATCH_BUDGET", "GETPBMNUMBER");
            parms.Desc01 = string.IsNullOrWhiteSpace(generationNo) ? "%" : generationNo;
            parms.Desc02 = ProductFilter(type);
            return await unitOfWork.SP_Call.ListAsync<ProductionBudget>(parms);
        }

        public async Task<IEnumerable<BatchCode>> GetBatches(string type)
        {
            if (common.GetComcod().StartsWith("61", StringComparison.Ordinal)) return Enumerable.Empty<BatchCode>();
            var parms = Params("SP_ENTRY_BATCH_BUDGET", "GETBATCHCODEREQ");
            parms.Desc01 = "%";
            parms.Desc02 = type == "Entry" ? "" : type == "EntrySemi" ? "Semi" : "SMT";
            return await unitOfWork.SP_Call.ListAsync<BatchCode>(parms);
        }

        public async Task<IEnumerable<PreviousDpr>> GetPreviousDprs(string date, string budgetNo)
        {
            var parms = Params("SP_ENTRY_BATCH_BUDGET", "GETDPRNOLIST");
            parms.Desc01 = date;
            parms.Desc02 = budgetNo;
            return await unitOfWork.SP_Call.ListAsync<PreviousDpr>(parms);
        }

        public async Task<string> GetNextDprNo(string date)
        {
            var parms = Params("SP_ENTRY_BATCH_BUDGET", "GETDPRNO");
            parms.Desc01 = date;
            return (await unitOfWork.SP_Call.ListAsync<DprNumber>(parms)).FirstOrDefault()?.dprno ?? "";
        }

        public async Task<RequisitionDataResponse> GetRequisitionData(string type, string budgetNo, string dprNo, string date)
        {
            var callType = common.GetComcod() == "6101" ? "RPTBATCHBUDGET_02" : "RPTBATCHBUDGET_01";
            var parms = Params("SP_ENTRY_BATCH_BUDGET", callType);
            parms.Desc01 = budgetNo;
            parms.Desc02 = dprNo;
            parms.Desc03 = ProductFilter(type);
            parms.Desc04 = date;
            var result = await unitOfWork.SP_Call.ListAsync<RequisitionProduct, RequisitionMaterial>(parms);
            return new RequisitionDataResponse { products = result.Item1, materials = result.Item2 };
        }

        public async Task<IEnumerable<RequisitionMaterial>> GenerateMaterials(string type, string budgetNo, List<RequisitionProduct> products)
        {
            var table = UtilityList.ListToDataTable(products);
            table.TableName = "tbl1";
            var data = new DataSet("ds1");
            data.Tables.Add(table);
            var parms = Params("SP_ENTRY_PRO_QC_INFO", "SHOWREPRODRES");
            parms.Dxml01 = data;
            parms.Desc01 = ProductFilter(type);
            parms.Desc02 = budgetNo;
            return await unitOfWork.SP_Call.ListAsync<RequisitionMaterial>(parms);
        }

        public async Task<bool> Save(SaveProductionRequisitionRequest request)
        {
            if (request.products.Any(x => x.cresqty > x.bbqty)) return false;

            var userId = common.GetUserId();
            var postedBy = common.GetComcod() + (userId.Length <= 3 ? userId : userId[^3..]);
            var terminal = common.GetIPAddress();
            var session = "";//httpContextAccessor.HttpContext?.TraceIdentifier ?? "";
            var postedDate = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

            var header = Params("SP_ENTRY_BATCH_BUDGET", "INORUPDATEPBREQ");
            header.Desc01 = "PBREQB";
            header.Desc02 = request.dprNo;
            header.Desc03 = request.budgetNo;
            header.Desc04 = request.date;
            header.Desc05 = request.batchCode;
            header.Desc06 = postedBy;
            header.Desc07 = terminal;
            header.Desc08 = postedDate;
            header.Desc09 = session;
            if (!await unitOfWork.SP_Call.ExecuteAsync(header)) return false;

            foreach (var item in request.products)
            {
                var detail = Params("SP_ENTRY_BATCH_BUDGET", "INORUPDATEPBREQ");
                detail.Desc01 = "PBREQA";
                detail.Desc02 = request.dprNo;
                detail.Desc03 = item.procode;
                detail.Desc04 = item.rsircode;
                detail.Desc05 = item.cresqty.ToString(CultureInfo.InvariantCulture);
                detail.Desc06 = item.spcfcod;
                if (!await unitOfWork.SP_Call.ExecuteAsync(detail)) return false;
            }

            foreach (var item in request.materials)
            {
                var material = Params("SP_ENTRY_BATCH_BUDGET", "INORUPDATEPBREQB");
                material.Desc01 = request.dprNo;
                material.Desc02 = item.procode;
                material.Desc03 = item.rsircode;
                material.Desc04 = item.cresqty.ToString(CultureInfo.InvariantCulture);
                material.Desc05 = item.spcfcod;
                if (!await unitOfWork.SP_Call.ExecuteAsync(material)) return false;
            }

            var settings = (await unitOfWork.SP_Call.ListAsync<CompanyProductionSettings>(Params("SP_GEN_INFO", "COMPANY_GENERAL_INFO"))).FirstOrDefault();
            var autoStart = request.type == "Entry" ? settings?.useprdprc == true : request.type == "EntrySemi" && settings?.useprdprcsemi == true;
            return !autoStart || await AutoStartProduction(request, userId, terminal, session);
        }

        private async Task<bool> AutoStartProduction(SaveProductionRequisitionRequest request, string userId, string terminal, string session)
        {
            var start = Params("SP_ENTRY_PRODUCTION_PROCESS", "STARTPROCESS");
            start.Desc01 = request.batchCode;
            start.Desc02 = request.dprNo;
            var rows = (await unitOfWork.SP_Call.ListAsync<ProductionStartRow>(start)).ToList();
            if (rows.Count == 0) return true;

            var batchCode = rows[0].batchcode?.Trim() ?? request.batchCode;
            var last = Params("SP_ENTRY_PRODUCTION_PROCESS", "GETLASTPPN");
            last.Desc01 = batchCode;
            var suffix = (await unitOfWork.SP_Call.ListAsync<LastProductionProcessNumber>(last)).FirstOrDefault()?.ppnno;
            if (string.IsNullOrWhiteSpace(suffix)) suffix = "00001";
            var date = DateTime.Parse(request.date, CultureInfo.InvariantCulture);
            var ppnNo = $"PPN{date:yyyyMM}{suffix}";

            foreach (var row in rows)
            {
                var detail = Params("SP_ENTRY_PRODUCTION_PROCESS", "INSERTORUPPROTRANS");
                detail.Desc01 = batchCode;
                detail.Desc02 = row.fprocscode.ToString(CultureInfo.InvariantCulture);
                detail.Desc03 = row.tprocscode?.Trim() ?? "";
                detail.Desc04 = row.rsircode?.Trim() ?? "";
                detail.Desc05 = DateTime.TryParse(row.pbdate, out var received) ? received.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) : request.date;
                detail.Desc06 = row.rsqty.ToString(CultureInfo.InvariantCulture);
                detail.Desc08 = "tbla";
                detail.Desc09 = ppnNo;
                detail.Desc10 = request.dprNo;
                detail.Desc11 = userId;
                detail.Desc12 = terminal;
                detail.Desc13 = session;
                if (!await unitOfWork.SP_Call.ExecuteAsync(detail)) return false;
            }

            var header = Params("SP_ENTRY_PRODUCTION_PROCESS", "INSERTORUPPROTRANS");
            header.Desc01 = ppnNo;
            header.Desc02 = batchCode;
            header.Desc03 = "000000000000";
            header.Desc04 = "000000000000";
            header.Desc05 = request.date;
            header.Desc06 = userId;
            header.Desc07 = terminal;
            header.Desc08 = "tblb";
            header.Desc09 = session;
            return await unitOfWork.SP_Call.ExecuteAsync(header);
        }

        private ClassProAccessParams Params(string procedure, string callType) => new()
        {
            Comp1 = common.GetComcod(),
            StoredProcedure = procedure,
            Calltype = callType
        };

        private static string ProductFilter(string type) => type == "Entry" ? "41%" : type == "EntrySemi" ? "0101999%" : "0101998%";
    }
}
