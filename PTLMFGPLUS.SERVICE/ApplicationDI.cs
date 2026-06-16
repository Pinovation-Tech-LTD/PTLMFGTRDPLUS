using Microsoft.Extensions.DependencyInjection;
using PTLMFGPLUS.SERVICE.ControlPanel;
using PTLMFGPLUS.SERVICE.S_07_RM;
using PTLMFGPLUS.SERVICE.S_13_ProdMon;



namespace PTLMFGPLUS.SERVICE
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<Icomperm, compermService>();
            services.AddScoped<IPurMTReqService, PurMTReqService>();
            services.AddScoped<IRawMattInterfaceService, RawMattInterfaceService>();
            services.AddScoped< IPurMTReqGatePassService, PurMTReqGatePassService>();
            services.AddScoped<IMaterialsTransferService, MaterialsTransferService>();
            services.AddScoped<IProductionInterfaceService, ProductionInterfaceService>();
            return services;
        }
    }
}
