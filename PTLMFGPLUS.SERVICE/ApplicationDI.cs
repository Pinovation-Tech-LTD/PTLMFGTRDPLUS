using Microsoft.Extensions.DependencyInjection;
using PTLMFGPLUS.SERVICE.ControlPanel;
using PTLMFGPLUS.SERVICE.S_07_RM;



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
            return services;
        }
    }
}
