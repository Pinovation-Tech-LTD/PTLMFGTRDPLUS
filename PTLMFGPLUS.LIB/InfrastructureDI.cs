using Microsoft.Extensions.DependencyInjection;
using PTLMFGPLUS.LIB.Repository;
using PTLMFGPLUS.LIB.Repository.IRepository;

namespace PTLMFGPLUS.LIB
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
