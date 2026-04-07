using Microsoft.Extensions.DependencyInjection;
using PTLMFGPLUS.SERVICE.ControlPanel;



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
            return services;
        }
    }
}
