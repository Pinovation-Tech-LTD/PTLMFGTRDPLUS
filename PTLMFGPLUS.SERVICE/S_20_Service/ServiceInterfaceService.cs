using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.E_20_Service.EServiceInterface;

namespace PTLMFGPLUS.SERVICE.S_20_Service
{
    public interface IServiceInterfaceService
    {
        Exception GetError();
        Task<ServiceInterfaceInitialResponse> GetInitialData();
        Task<ServiceInterfaceResponse> GetInterfaceInfo(
            DateTime fromDate,
            DateTime toDate,
            string centerCode,
            string serviceType,
            string imei);
    }

    public class ServiceInterfaceService(
        ICommonService common,
        IUnitOfWork unitOfWork) : IServiceInterfaceService
    {
        public Exception GetError()
        {
            return common.GetError();
        }

        public async Task<ServiceInterfaceInitialResponse> GetInitialData()
        {
            string companyCode = common.GetComcod();
            DateTime today = DateTime.Today;
            DateTime fromDate = companyCode.StartsWith("73", StringComparison.Ordinal)
                || companyCode == "8401"
                    ? today
                    : today.AddDays(-7);
            var response = new ServiceInterfaceInitialResponse
            {
                defaultFromDate = fromDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
                defaultToDate = today.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)
            };

            var centerParameters = new ClassProAccessParams
            {
                StoredProcedure = "dbo_Service.SP_ENTRY_PRODUCT_RECEIVE",
                Calltype = "GETSERVCENTERLIST",
                Comp1 = companyCode,
                Desc01 = common.GetUserId(),
                Desc02 = "%"
            };
            var centerResult = await unitOfWork.SP_Call.DataSetAsync(centerParameters);

            if (centerResult?.Tables.Count > 0)
            {
                response.serviceCenters = centerResult.Tables[0]
                    .DataTableToList<ServiceCenterOption>();
            }

            response.serviceCenters.Add(new ServiceCenterOption
            {
                actcode = "000000000000",
                actdesc = "All centers"
            });

            var typeParameters = new ClassProAccessParams
            {
                StoredProcedure = "dbo_Service.SP_ENTRY_PRODUCT_RECEIVE",
                Calltype = "GETPROBLIST",
                Comp1 = companyCode
            };
            var typeResult = await unitOfWork.SP_Call.DataSetAsync(typeParameters);

            if (typeResult?.Tables.Count > 0)
            {
                response.serviceTypes = typeResult.Tables[0]
                    .DataTableToList<ServiceTypeOption>()
                    .Where(item => item.gcod.StartsWith("03", StringComparison.Ordinal))
                    .ToList();
            }

            response.serviceTypes.Add(new ServiceTypeOption
            {
                gcod = "0000000",
                gdesc = "All types"
            });

            return response;
        }

        public async Task<ServiceInterfaceResponse> GetInterfaceInfo(
            DateTime fromDate,
            DateTime toDate,
            string centerCode,
            string serviceType,
            string imei)
        {
            var parameters = new ClassProAccessParams
            {
                StoredProcedure = "dbo_Service.SP_REPORT_SERVICE_INTERFACE",
                Calltype = "GETSERVINTERFACENEW",
                Comp1 = common.GetComcod(),
                Desc01 = fromDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
                Desc02 = toDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
                Desc03 = common.GetUserId(),
                Desc04 = centerCode == "000000000000" ? "%" : centerCode,
                Desc05 = serviceType == "0000000" ? "%" : serviceType,
                Desc06 = imei
            };
            var result = await unitOfWork.SP_Call.DataSetAsync(parameters);
            var response = new ServiceInterfaceResponse();

            if (result?.Tables.Count > 0)
            {
                response.records = result.Tables[0]
                    .DataTableToList<ServiceInterfaceRecord>();
            }

            if (result?.Tables.Count > 1)
            {
                response.summary = result.Tables[1]
                    .DataTableToList<ServiceInterfaceSummary>()
                    .FirstOrDefault() ?? new ServiceInterfaceSummary();
            }

            return response;
        }
    }
}
