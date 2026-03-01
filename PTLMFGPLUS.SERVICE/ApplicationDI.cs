using Microsoft.Extensions.DependencyInjection;



namespace PTLMFGPLUS.SERVICE
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IPageRegistry, PageRegistry>();
            return services;
        }
    }
}
