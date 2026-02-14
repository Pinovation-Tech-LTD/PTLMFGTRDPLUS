
using Microsoft.Extensions.DependencyInjection;
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.SERVICE;


namespace PTLMFGPLUS.WEB
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services)
        {
            services.AddSingleton<LicenseService>();
            services.AddInfrastructureDI().AddApplicationDI();
            return services;
        }
    }
}
