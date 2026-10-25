using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_20_Service.ERecProductEntry;

namespace PTLMFGPLUS.SERVICE.S_20_Service
{
    public interface IRecProductEntryService
    {
        Task<InitialResponse> GetInitialData();
        Task<string> GetReceiveNo(string date, string centerId);
        Task<ProductResponse> GetProduct(string imei, bool editing);
        Task<EditResponse> GetEdit(string receiveNo, string centerId, string imei);
        Task<bool> Save(SaveRequest request);
    }

    public class RecProductEntryService(ICommonService common, IUnitOfWork unitOfWork) : IRecProductEntryService
    {
        private const string ReceiveSp = "dbo_Service.SP_ENTRY_PRODUCT_RECEIVE";

        public async Task<InitialResponse> GetInitialData()
        {
            var now = DateTime.Now;
            var response = new InitialResponse
            {
                companyCode = common.GetComcod(),
                entryDate = now.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
                probableDeliveryDate = now.AddDays(7).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
                activationDate = now.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
                bookingDate = now.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)
            };

            response.serviceCenters = await Lookup(ReceiveSp, "GETSERVCENTERLIST", common.GetUserId(), "%", "actcode", "actdesc");
            var master = await unitOfWork.SP_Call.ListAsync<ProblemRow>(Params(ReceiveSp, "GETPROBLIST"));
            response.serviceTypes = master.Where(x => x.gcod.StartsWith("03")).Select(ToLookup).ToList();
            response.problems = master.Where(x => x.gcod.StartsWith("01")).Select(ToLookup).ToList();
            response.accessories = master.Where(x => x.gcod.StartsWith("05")).Select(ToLookup).ToList();
            response.transports = await Lookup("SP_ENTRY_SALES_ORDER", "GETTRANSPORT", "%%", "", "sircode", "sirdesc");
            response.mechanics = await Lookup("SP_REPORT_LOGSTAUTS", "GETUSERNAME_SRV", "%%", "", "usrid", "usrsname");
            response.customers = (await unitOfWork.SP_Call.ListAsync<CustomerOption>(Params("SP_ENTRY_CODEBOOK02", "GETCRINF", "51%", "%", "%", "%"))).ToList();
            if (!response.customers.Any(x => x.sircode == "000000000000"))
            {
                response.customers.Insert(0, new CustomerOption { sircode = "000000000000", sirdesc = "Select customer" });
            }
            return response;
        }

        public async Task<string> GetReceiveNo(string date, string centerId)
        {
            if (!await HasCenter(centerId)) return "";
            var rows = await unitOfWork.SP_Call.ListAsync<ReceiveNumber>(Params(ReceiveSp, "LASTPROWATRECVNO", date, centerId));
            return rows.FirstOrDefault()?.maxno ?? "";
        }

        public async Task<ProductResponse> GetProduct(string imei, bool editing)
        {
            if (!editing && await IsOpenReceive(imei)) throw new InvalidOperationException("Product already received.");
            var product = (await unitOfWork.SP_Call.ListAsync<ProductInfo>(Params(ReceiveSp, "GETPRODWARRANTY", imei))).FirstOrDefault();
            if (product == null) throw new InvalidOperationException("Product not found.");
            var response = new ProductResponse { product = product };
            if (!string.IsNullOrWhiteSpace(product.bounceset))
            {
                var history = await unitOfWork.SP_Call.DataSetAsync(Params("dbo_service.SP_REPORT_PRODUCT_RECEIVE_02", "GETIMIESTATUS", product.mimei, "bounc", ""));
                if (history.Tables.Count > 0)
                {
                    response.history = history.Tables[0].AsEnumerable().Select(row => (object)history.Tables[0].Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => row[col] == DBNull.Value ? null : row[col])).ToList();
                }
            }
            return response;
        }

        public async Task<EditResponse> GetEdit(string receiveNo, string centerId, string imei)
        {
            if (!await HasCenter(centerId)) throw new UnauthorizedAccessException();
            var data = await unitOfWork.SP_Call.DataSetAsync(Params(ReceiveSp, "GETWARRANTYRECVIMEI", receiveNo, centerId));
            if (data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0) throw new InvalidOperationException("Receive record was not found.");
            var response = new EditResponse
            {
                header = data.Tables[0].DataTableToList<ReceiveHeader>().First(),
                lines = data.Tables.Count > 1 ? data.Tables[1].DataTableToList<ReceiveLine>() : new List<ReceiveLine>()
            };
            response.product = (await GetProduct(string.IsNullOrWhiteSpace(imei) ? response.header.mimei : imei, true)).product;
            return response;
        }

        public async Task<bool> Save(SaveRequest request)
        {
            bool editing = request.type.Equals("Edit", StringComparison.OrdinalIgnoreCase);
            if (!await HasCenter(request.centerId)) throw new UnauthorizedAccessException();
            if (!editing)
            {
                var businessDateRows = await unitOfWork.SP_Call.ListAsync<BusinessDate>(Params("SP_ENTRY_ACCOUNTS_PAYMENT", "GETBDATE"));
                DateTime businessDate = businessDateRows.FirstOrDefault()?.bdate.Date ?? DateTime.Today;
                DateTime receiveDate = DateTime.ParseExact(request.receiveDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                if (!(businessDate < receiveDate && receiveDate <= DateTime.Today))
                {
                    throw new InvalidOperationException("Entry date must be after the business date and cannot be in the future.");
                }
            }
            var product = (await unitOfWork.SP_Call.ListAsync<ProductInfo>(Params(ReceiveSp, "GETPRODWARRANTY", request.imei))).FirstOrDefault()
                ?? throw new InvalidOperationException("Product not found.");
            if (!editing && await IsOpenReceive(product.mimei)) throw new InvalidOperationException("Product already received.");
            var serviceTypes = await unitOfWork.SP_Call.ListAsync<ProblemRow>(Params(ReceiveSp, "GETPROBLIST"));
            var selectedType = serviceTypes.FirstOrDefault(x => x.gcod == request.serviceType && x.gcod.StartsWith("03"))
                ?? throw new InvalidOperationException("Select a valid service type.");
            if (!editing && selectedType.gval > 0 && product.scount >= selectedType.gval) throw new InvalidOperationException("The selected service type is not available for this product.");

            string receiveNo = editing ? request.previousReceiveNo : await GetReceiveNo(request.receiveDate, request.centerId);
            if (string.IsNullOrWhiteSpace(receiveNo)) throw new InvalidOperationException("Receive number could not be generated.");
            var postedDate = string.IsNullOrWhiteSpace(common.GetPostedDate()) ? DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture) : common.GetPostedDate();
            var xml = new DataSet("ds1");
            var table = UtilityList.ListToDataTable(request.lines ?? new List<ReceiveLine>());
            table.TableName = "tbl1";
            xml.Tables.Add(table);
            var p = Params(ReceiveSp, "INSERTSERVINFO");
            p.Dxml01 = xml;
            p.Desc01 = receiveNo; p.Desc02 = request.centerId; p.Desc03 = product.mimei; p.Desc04 = product.simei;
            p.Desc05 = product.rescod; p.Desc06 = request.receiveDate; p.Desc07 = request.customerName; p.Desc08 = request.customerMobile;
            p.Desc09 = request.customerEmail; p.Desc10 = request.customerAddress; p.Desc11 = request.customerComplaint;
            p.Desc12 = editing ? "" : common.GetUserId(); p.Desc13 = editing ? "" : common.GetIPAddress();
            p.Desc14 = editing ? "" : common.GetTerminalId(); p.Desc15 = postedDate; p.Desc16 = request.probableDeliveryDate;
            p.Desc17 = request.serviceType; p.Desc18 = DateTime.ParseExact(request.activationDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            p.Desc19 = request.missingItems; p.Desc20 = request.transportCode; p.Desc21 = request.consignmentNo; p.Desc22 = request.bookingDate;
            p.Desc23 = "Ok"; p.Desc24 = product.sdelno; p.Desc25 = request.customerMobile2; p.Desc26 = request.customerId;
            p.Desc27 = product.batchcode; p.Desc28 = request.odometer; p.Desc29 = request.registrationNo; p.Desc30 = request.mechanicId;
            p.Desc31 = product.scustid; p.Desc32 = request.advance.ToString(CultureInfo.InvariantCulture);
            bool saved = await unitOfWork.SP_Call.ExecuteAsync(p);
            if (saved && request.advance > 0)
            {
                var advance = Params(ReceiveSp, "INSERT_SER_ADVANCE_MR", request.centerId, receiveNo, request.receiveDate, "000000000000", common.GetIPAddress(), common.GetUserId(), common.GetTerminalId(), postedDate);
                saved = await unitOfWork.SP_Call.ExecuteAsync(advance);
            }
            return saved;
        }

        private async Task<bool> HasCenter(string centerId)
        {
            var centers = await Lookup(ReceiveSp, "GETSERVCENTERLIST", common.GetUserId(), "%", "actcode", "actdesc");
            return centers.Any(x => x.code == centerId);
        }

        private async Task<bool> IsOpenReceive(string imei)
        {
            var data = await unitOfWork.SP_Call.DataSetAsync(Params(ReceiveSp, "CHECKIMEIDEL", imei));
            return data.Tables.Count > 0 && data.Tables[0].AsEnumerable().Any(row => string.IsNullOrWhiteSpace(Convert.ToString(row["qcid"])));
        }

        private async Task<List<LookupItem>> Lookup(string sp, string call, string desc1, string desc2, string codeColumn, string textColumn)
        {
            var data = await unitOfWork.SP_Call.DataSetAsync(Params(sp, call, desc1, desc2));
            if (data.Tables.Count == 0) return new List<LookupItem>();
            return data.Tables[0].AsEnumerable().Select(row => new LookupItem { code = Convert.ToString(row[codeColumn]) ?? "", description = Convert.ToString(row[textColumn]) ?? "" }).ToList();
        }

        private ClassProAccessParams Params(string sp, string call, params string[] values)
        {
            var p = new ClassProAccessParams { Comp1 = common.GetComcod(), StoredProcedure = sp, Calltype = call };
            for (int index = 0; index < values.Length; index++) typeof(ClassProAccessParams).GetProperty($"Desc{index + 1:00}")?.SetValue(p, values[index] ?? "");
            return p;
        }

        private static LookupItem ToLookup(ProblemRow item) => new() { code = item.gcod, description = item.gdesc, value = item.gval };
        private class ProblemRow { public string gcod { get; set; } = ""; public string gdesc { get; set; } = ""; public double gval { get; set; } }
        private class ReceiveNumber { public string maxno { get; set; } = ""; }
        private class BusinessDate { public DateTime bdate { get; set; } }
    }
}
