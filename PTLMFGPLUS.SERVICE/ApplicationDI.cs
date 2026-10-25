using Microsoft.Extensions.DependencyInjection;
using PTLMFGPLUS.SERVICE.ControlPanel;
using PTLMFGPLUS.SERVICE.S_07_RM;
using PTLMFGPLUS.SERVICE.S_13_ProdMon;
using PTLMFGPLUS.SERVICE.S_11_Pro;
using PTLMFGPLUS.SERVICE.S_07_Inv;
using PTLMFGPLUS.SERVICE.S_20_Service;



namespace PTLMFGPLUS.SERVICE
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IStepOfOperationService, StepOfOperationService>();
            services.AddScoped<IPagePermissionService, PagePermissionService>();
            services.AddScoped<Icomperm, compermService>();
            services.AddScoped<IUserPermissionService, UserPermissionService>();
            
            //MATERIAL TRANSFER INTERFACE
            services.AddScoped<IPurMTReqService, PurMTReqService>();
            services.AddScoped<IRawMattInterfaceService, RawMattInterfaceService>();
            services.AddScoped< IPurMTReqGatePassService, PurMTReqGatePassService>();
            services.AddScoped<IMaterialsTransferService, MaterialsTransferService>();

            //PRODUCTION INTERFACE
            services.AddScoped<IProductionInterfaceService, ProductionInterfaceService>();
            services.AddScoped<IProdBudgetService, ProdBudgetService>();

            //PRODUCTION REQUISITION
            services.AddScoped<IProdReqService, ProdReqService>();
            services.AddScoped<IPBMatIssueSingleService, PBMatIssueSingleService>();

            //SERVICE INTERFACE
            services.AddScoped<IServiceInterfaceService, ServiceInterfaceService>();
            services.AddScoped<IRecProductEntryService, RecProductEntryService>();
            services.AddScoped<IAllRecProductService, AllRecProductService>();
            services.AddScoped<IDiagProductEntryService, DiagProductEntryService>();


            return services;
        }
    }
}
