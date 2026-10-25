using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_20_Service.EDiagProductEntry;

namespace PTLMFGPLUS.SERVICE.S_20_Service
{
    public interface IDiagProductEntryService
    {
        Task<LoadResponse> Load(string centerId, string receiveNo, string imei, string date);
        Task<List<LookupItem>> GetSpareOptions(string centerId, string date);
        Task<bool> SaveDiagnosis(SaveRequest request);
        Task<bool> Approve(WorkflowRequest request);
        Task<bool> QcPass(WorkflowRequest request);
        Task<bool> QcReject(WorkflowRequest request);
        Task<bool> ServiceDone(WorkflowRequest request);
    }

    public class DiagProductEntryService(ICommonService common, IUnitOfWork unitOfWork) : IDiagProductEntryService
    {
        private const string ReceiveSp = "dbo_Service.SP_ENTRY_PRODUCT_RECEIVE";

        public async Task<LoadResponse> Load(string centerId, string receiveNo, string imei, string date)
        {
            await EnsureCenter(centerId);
            var data = await unitOfWork.SP_Call.DataSetAsync(Params(ReceiveSp, "GETWARRANTYRECVIMEI", receiveNo, centerId));
            if (data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0)
            {
                throw new InvalidOperationException("Receive record was not found.");
            }

            var response = new LoadResponse
            {
                companyCode = common.GetComcod(),
                header = MapHeader(data.Tables[0].Rows[0])
            };
            string productImei = string.IsNullOrWhiteSpace(imei) ? response.header.mimei : imei;
            response.product = (await unitOfWork.SP_Call.ListAsync<ProductInfo>(Params(ReceiveSp, "GETPRODWARRANTY", productImei))).FirstOrDefault() ?? new ProductInfo();

            var receivedLines = data.Tables.Count > 1 ? MapProblems(data.Tables[1]) : new List<ProblemLine>();
            response.receivedProblems = receivedLines.Where(item => item.grp == "A").ToList();
            response.accessories = receivedLines.Where(item => item.grp == "B").ToList();
            response.diagnosisProblems = data.Tables.Count > 2 && data.Tables[2].Rows.Count > 0
                ? MapProblems(data.Tables[2])
                : response.receivedProblems.Select(CloneProblem).ToList();
            response.spareParts = data.Tables.Count > 3 ? MapParts(data.Tables[3]) : new List<SparePart>();
            response.pendingParts = data.Tables.Count > 4 ? MapParts(data.Tables[4]) : new List<SparePart>();

            var master = await unitOfWork.SP_Call.ListAsync<ProblemOption>(Params(ReceiveSp, "GETPROBLIST"));
            response.problemOptions = master.Where(item => item.gcod.StartsWith("01")).Select(ToLookup).ToList();
            response.warrantyOptions = master.Where(item => item.gcod.StartsWith("03")).Select(ToLookup).ToList();
            response.spareOptions = await GetSpareOptions(centerId, string.IsNullOrWhiteSpace(date) ? DateTime.Today.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) : date);
            return response;
        }

        public async Task<List<LookupItem>> GetSpareOptions(string centerId, string date)
        {
            await EnsureCenter(centerId);
            var data = await unitOfWork.SP_Call.DataSetAsync(Params(ReceiveSp, "GETSPAREPARTSINFO", centerId, date, date));
            if (data.Tables.Count == 0)
            {
                return new List<LookupItem>();
            }
            return data.Tables[0].AsEnumerable().Select(row => new LookupItem
            {
                code = Text(row, "rsircode", "sircode"),
                description = Text(row, "sirdesc"),
                unit = Text(row, "sirunit"),
                specificationCode = Text(row, "spcfcod"),
                specification = Text(row, "spcfdesc"),
                value = Number(row, "sirval"),
                stockQuantity = Number(row, "stkqty")
            }).ToList();
        }

        public async Task<bool> SaveDiagnosis(SaveRequest request)
        {
            await EnsureCenter(request.centerId);
            DateTime diagnosisDate = ParseDate(request.diagnosisDate, "Diagnosis date");
            DateTime receivedDate = ParseDate(request.receivedDate, "Receive date");
            if (diagnosisDate < receivedDate)
            {
                throw new InvalidOperationException("Diagnosis date cannot be earlier than the receive date.");
            }
            if (common.GetComcod() != "8421" && (request.spareParts == null || request.spareParts.Count == 0))
            {
                throw new InvalidOperationException("Add at least one spare part.");
            }

            ValidateParts(request.spareParts);
            var current = await GetHeader(request.centerId, request.receiveNo);
            bool entryMode = request.type.Equals("DiagEnrty", StringComparison.OrdinalIgnoreCase);
            var parameters = Params(ReceiveSp, "INSERTDIGNINFO");
            parameters.Dxml01 = Xml(request.problems ?? new List<ProblemLine>(), "tbl1");
            parameters.Dxml02 = Xml(request.spareParts ?? new List<SparePart>(), "tbl2");
            parameters.Desc01 = request.receiveNo;
            parameters.Desc02 = request.centerId;
            parameters.Desc03 = request.productCode;
            parameters.Desc04 = request.diagnosisDate;
            parameters.Desc05 = entryMode ? common.GetUserId() : current.dignbyid1;
            parameters.Desc06 = entryMode ? common.GetTerminalId() : current.digntrmid1;
            parameters.Desc07 = entryMode ? common.GetIPAddress() : current.dignseson1;
            parameters.Desc08 = entryMode ? "" : common.GetUserId();
            parameters.Desc09 = entryMode ? "" : common.GetTerminalId();
            parameters.Desc10 = entryMode ? "" : common.GetIPAddress();
            parameters.Desc11 = entryMode ? "" : Now();
            parameters.Desc12 = request.warrantyCode;
            parameters.Desc13 = "";
            parameters.Desc14 = request.remarks;
            return await unitOfWork.SP_Call.ExecuteAsync(parameters);
        }

        public async Task<bool> Approve(WorkflowRequest request)
        {
            await EnsureCenter(request.centerId);
            var approved = (request.spareParts ?? new List<SparePart>()).Where(item => IsApproved(item.apstatus)).ToList();
            var p = Params(ReceiveSp, "APPROVEDPRODUCT", request.receiveNo, request.centerId, common.GetUserId(), common.GetIPAddress(), common.GetTerminalId(), request.diagnosisDate, Now());
            p.Dxml02 = Xml(approved, "tbl2");
            return await unitOfWork.SP_Call.ExecuteAsync(p);
        }

        public async Task<bool> QcPass(WorkflowRequest request)
        {
            await EnsureCenter(request.centerId);
            DateTime qcDate = ParseDate(request.date, "QC date");
            DateTime diagnosisDate = ParseDate(request.diagnosisDate, "Diagnosis date");
            if (qcDate < diagnosisDate)
            {
                throw new InvalidOperationException("QC date cannot be earlier than the diagnosis date.");
            }
            var issueParts = (request.spareParts ?? new List<SparePart>()).Where(item => item.execqty > 0).ToList();
            var number = (await unitOfWork.SP_Call.ListAsync<ExecutionNumber>(Params(ReceiveSp, "LASTEXECNO", request.date, request.centerId))).FirstOrDefault()?.maxno ?? "";
            if (string.IsNullOrWhiteSpace(number))
            {
                throw new InvalidOperationException("Issue number could not be generated.");
            }
            var issue = Params(ReceiveSp, "DIGNISSUEPRODINSERT", request.centerId, request.receiveNo, number, Now(), common.GetUserId(), common.GetTerminalId(), common.GetIPAddress(), request.diagnosisDate);
            issue.Dxml01 = Xml(issueParts, "tbl1");
            if (!await unitOfWork.SP_Call.ExecuteAsync(issue))
            {
                return false;
            }
            return await unitOfWork.SP_Call.ExecuteAsync(Params(ReceiveSp, "PROQCCOMP", request.receiveNo, request.centerId, "Ok", common.GetUserId(), Now(), common.GetTerminalId(), common.GetIPAddress()));
        }

        public async Task<bool> QcReject(WorkflowRequest request)
        {
            await EnsureCenter(request.centerId);
            return await unitOfWork.SP_Call.ExecuteAsync(Params(ReceiveSp, "PROQCREJECT", request.receiveNo, request.centerId, Now(), common.GetUserId(), request.type));
        }

        public async Task<bool> ServiceDone(WorkflowRequest request)
        {
            await EnsureCenter(request.centerId);
            DateTime doneDate = ParseDate(request.date, "Service done date");
            DateTime diagnosisDate = ParseDate(request.diagnosisDate, "Diagnosis date");
            if (doneDate < diagnosisDate)
            {
                throw new InvalidOperationException("Service done date cannot be earlier than the diagnosis date.");
            }
            if ((request.spareParts ?? new List<SparePart>()).Any(item => !IsApproved(item.apstatus)))
            {
                throw new InvalidOperationException("All spare parts must be approved before service completion.");
            }
            if ((request.spareParts ?? new List<SparePart>()).Any(item => item.rsircode.StartsWith("4299")) && string.IsNullOrWhiteSpace(request.replacementImei))
            {
                throw new InvalidOperationException("Replacement IMEI is required.");
            }
            return await unitOfWork.SP_Call.ExecuteAsync(Params(ReceiveSp, "PROWORKCOMP", request.receiveNo, request.centerId, "Ok", common.GetUserId(), Now(), request.replacementImei));
        }

        private async Task<Header> GetHeader(string centerId, string receiveNo)
        {
            var data = await unitOfWork.SP_Call.DataSetAsync(Params(ReceiveSp, "GETWARRANTYRECVIMEI", receiveNo, centerId));
            if (data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0) throw new InvalidOperationException("Receive record was not found.");
            return MapHeader(data.Tables[0].Rows[0]);
        }

        private async Task EnsureCenter(string centerId)
        {
            var data = await unitOfWork.SP_Call.DataSetAsync(Params(ReceiveSp, "GETSERVCENTERLIST", common.GetUserId(), "%"));
            bool allowed = data.Tables.Count > 0 && data.Tables[0].AsEnumerable().Any(row => Text(row, "actcode") == centerId);
            if (!allowed) throw new UnauthorizedAccessException();
        }

        private static void ValidateParts(IEnumerable<SparePart> parts)
        {
            foreach (var part in parts ?? Enumerable.Empty<SparePart>())
            {
                if (part.qty <= 0) throw new InvalidOperationException($"Quantity must be greater than zero for {part.sirdesc}.");
                if (!part.rsircode.StartsWith("4297") && !part.rsircode.StartsWith("4299") && part.qty > part.stkqty)
                    throw new InvalidOperationException($"Quantity exceeds available stock for {part.sirdesc}.");
            }
        }

        private static DataSet Xml<T>(IEnumerable<T> items, string tableName)
        {
            var data = new DataSet();
            var table = UtilityList.ListToDataTable(items.ToList());
            table.TableName = tableName;
            data.Tables.Add(table);
            return data;
        }

        private ClassProAccessParams Params(string sp, string call, params string[] values)
        {
            var p = new ClassProAccessParams { Comp1 = common.GetComcod(), StoredProcedure = sp, Calltype = call };
            for (int index = 0; index < values.Length; index++) typeof(ClassProAccessParams).GetProperty($"Desc{index + 1:00}")?.SetValue(p, values[index] ?? "");
            return p;
        }

        private static Header MapHeader(DataRow row) => new()
        {
            recvno = Text(row, "recvno"), centrid = Text(row, "centrid"), actdesc = Text(row, "actdesc"), mimei = Text(row, "mimei"), simei = Text(row, "simei"),
            procode = Text(row, "procode", "rescod"), custname = Text(row, "custname"), custmob = Text(row, "custmob"), custmob2 = Text(row, "custmob2"),
            custmail = Text(row, "custmail"), custadd = Text(row, "custadd"), custop = Text(row, "custop"), hsmissing = Text(row, "hsmissing"),
            recvdate = DateText(row, "recvdate"), digndate = DateText(row, "digndate"), deldate = DateText(row, "deldate"), wrcode = Text(row, "wrcode"),
            rwrcode = Text(row, "rwrcode"), wrstatus = Text(row, "wrstatus"), rimei = Text(row, "rimei"), rsirdesc = Text(row, "rsirdesc"),
            custstatus = Text(row, "custstatus"), custhtmsg = Text(row, "custhtmsg"), dignbyid1 = Text(row, "dignbyid1"),
            digntrmid1 = Text(row, "digntrmid1"), dignseson1 = Text(row, "dignseson1")
        };

        private static List<ProblemLine> MapProblems(DataTable table) => table.AsEnumerable().Select(row => new ProblemLine
        {
            grp = Text(row, "grp"), gcod = Text(row, "gcod"), gdesc = Text(row, "gdesc"), remarks = Text(row, "remarks")
        }).ToList();

        private static List<SparePart> MapParts(DataTable table) => table.AsEnumerable().Select(row => new SparePart
        {
            rsircode = Text(row, "rsircode", "sircode"), sirdesc = Text(row, "sirdesc"), sirunit = Text(row, "sirunit"), qty = Number(row, "qty"),
            sirval = Number(row, "sirval"), amount = Number(row, "amount"), apstatus = Text(row, "apstatus"), execqty = Number(row, "execqty"),
            spcfcod = Text(row, "spcfcod"), spcfdesc = Text(row, "spcfdesc"), stkqty = Number(row, "stkqty")
        }).ToList();

        private static string Text(DataRow row, params string[] names)
        {
            string name = names.FirstOrDefault(item => row.Table.Columns.Contains(item));
            return name == null || row[name] == DBNull.Value ? "" : Convert.ToString(row[name], CultureInfo.InvariantCulture) ?? "";
        }

        private static double Number(DataRow row, string name) => row.Table.Columns.Contains(name) && row[name] != DBNull.Value ? Convert.ToDouble(row[name], CultureInfo.InvariantCulture) : 0;
        private static string DateText(DataRow row, string name) => DateTime.TryParse(Text(row, name), out DateTime value) ? value.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) : "";
        private static DateTime ParseDate(string value, string label) => DateTime.TryParseExact(value, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result) ? result : throw new InvalidOperationException($"{label} must use dd-MMM-yyyy format.");
        private static bool IsApproved(string value) => value.Equals("True", StringComparison.OrdinalIgnoreCase) || value.Equals("Ok", StringComparison.OrdinalIgnoreCase);
        private static string Now() => DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
        private static ProblemLine CloneProblem(ProblemLine item) => new() { grp = item.grp, gcod = item.gcod, gdesc = item.gdesc, remarks = item.remarks };
        private static LookupItem ToLookup(ProblemOption item) => new() { code = item.gcod, description = item.gdesc, value = item.gval };
        private class ProblemOption { public string gcod { get; set; } = ""; public string gdesc { get; set; } = ""; public double gval { get; set; } }
        private class ExecutionNumber { public string maxno { get; set; } = ""; }
    }
}
