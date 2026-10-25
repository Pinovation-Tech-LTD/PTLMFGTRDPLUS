using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_20_Service.EAllRecProduct;

namespace PTLMFGPLUS.SERVICE.S_20_Service
{
    public interface IAllRecProductService
    {
        Task<InitialResponse> GetInitialData(string date);
        Task<IEnumerable<ReceiveRecord>> GetRecords(string type, DateTime fromDate, DateTime toDate, string centerId, string receiveNo);
        Task<IEnumerable<LookupItem>> GetEmployees(string type);
        Task<bool> UpdateTransfer(UpdateRequest request);
        Task<bool> AssignEmployee(UpdateRequest request);
        Task<bool> UpdateHotline(UpdateRequest request);
    }

    public class AllRecProductService(ICommonService common, IUnitOfWork unitOfWork) : IAllRecProductService
    {
        private const string ReceiveSp = "dbo_Service.SP_ENTRY_PRODUCT_RECEIVE";

        public async Task<InitialResponse> GetInitialData(string date)
        {
            DateTime selectedDate = ParseDate(date) ?? DateTime.Today;
            DateTime fromDate = string.IsNullOrWhiteSpace(date)
                ? new DateTime(selectedDate.Year, selectedDate.Month, 1)
                : selectedDate;
            var response = new InitialResponse
            {
                fromDate = fromDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
                toDate = selectedDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)
            };
            var data = await unitOfWork.SP_Call.DataSetAsync(Params(ReceiveSp, "GETSERVCENTERLIST", common.GetUserId(), "%"));
            if (data.Tables.Count > 0)
            {
                response.serviceCenters = data.Tables[0].AsEnumerable().Select(row => new LookupItem
                {
                    code = Convert.ToString(row["actcode"]) ?? "",
                    description = Convert.ToString(row["actdesc"]) ?? ""
                }).ToList();
            }
            response.serviceCenters.Add(new LookupItem { code = "000000000000", description = "All centers" });
            return response;
        }

        public async Task<IEnumerable<ReceiveRecord>> GetRecords(string type, DateTime fromDate, DateTime toDate, string centerId, string receiveNo)
        {
            string centerPattern = string.IsNullOrWhiteSpace(centerId) || centerId == "000000000000" ? "%" : centerId + "%";
            string receivePattern = string.IsNullOrWhiteSpace(receiveNo) ? "%" : receiveNo + "%";
            var rows = (await unitOfWork.SP_Call.ListAsync<ReceiveRecord>(Params(
                ReceiveSp,
                "GETALLREVPRODUCT",
                fromDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
                toDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
                common.GetUserId(),
                centerPattern,
                receivePattern))).ToList();

            return type switch
            {
                "AssignEng" => rows.Where(row => row.trnstatus == "Ok"),
                "Transfer" => rows.Where(row => string.IsNullOrWhiteSpace(row.trnstatus)),
                "HotLine" => rows.Where(row => !string.IsNullOrWhiteSpace(row.custstatus)),
                _ => rows
            };
        }

        public async Task<IEnumerable<LookupItem>> GetEmployees(string type)
        {
            if (type == "HotLine")
            {
                var data = await unitOfWork.SP_Call.DataSetAsync(Params(ReceiveSp, "GETSERVCENTERLIST", "%", "%"));
                if (data.Tables.Count < 2) return Array.Empty<LookupItem>();
                return data.Tables[1].AsEnumerable().Select(row => new LookupItem
                {
                    code = Convert.ToString(row["gcod"]) ?? "",
                    description = Convert.ToString(row["gdesc"]) ?? ""
                }).ToList();
            }
            var employees = await unitOfWork.SP_Call.DataSetAsync(Params("SP_REPORT_LOGSTAUTS", "GETUSERNAME_SRV", "%%"));
            if (employees.Tables.Count == 0) return Array.Empty<LookupItem>();
            return employees.Tables[0].AsEnumerable().Select(row => new LookupItem
            {
                code = Convert.ToString(row["usrid"]) ?? "",
                description = Convert.ToString(row["usrsname"]) ?? ""
            }).ToList();
        }

        public Task<bool> UpdateTransfer(UpdateRequest request)
        {
            return ExecuteAuthorized(request, "UPDATEDOADAPTRANSFER", request.transferred ? "Ok" : "");
        }

        public Task<bool> AssignEmployee(UpdateRequest request)
        {
            return ExecuteAuthorized(request, "INSERASSIGNENGID", request.employeeId, Now());
        }

        public Task<bool> UpdateHotline(UpdateRequest request)
        {
            return ExecuteAuthorized(request, "PROHOTLINE", Now(), common.GetUserId(), request.employeeId, request.hotlineMessage, "Edit");
        }

        private async Task<bool> ExecuteAuthorized(UpdateRequest request, string callType, params string[] trailing)
        {
            if (!await HasCenterAccess(request.centerId)) throw new UnauthorizedAccessException();
            var values = new List<string> { request.centerId, request.receiveNo };
            if (callType == "PROHOTLINE") values = new List<string> { request.receiveNo, request.centerId };
            values.AddRange(trailing);
            return await unitOfWork.SP_Call.ExecuteAsync(Params(ReceiveSp, callType, values.ToArray()));
        }

        private async Task<bool> HasCenterAccess(string centerId)
        {
            var data = await unitOfWork.SP_Call.DataSetAsync(Params(ReceiveSp, "GETSERVCENTERLIST", common.GetUserId(), "%"));
            return data.Tables.Count > 0 && data.Tables[0].AsEnumerable().Any(row => Convert.ToString(row["actcode"]) == centerId);
        }

        private ClassProAccessParams Params(string sp, string call, params string[] values)
        {
            var p = new ClassProAccessParams { Comp1 = common.GetComcod(), StoredProcedure = sp, Calltype = call };
            for (int index = 0; index < values.Length; index++) typeof(ClassProAccessParams).GetProperty($"Desc{index + 1:00}")?.SetValue(p, values[index] ?? "");
            return p;
        }

        private static DateTime? ParseDate(string value) => DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result) ? result : null;
        private static string Now() => DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
    }
}
