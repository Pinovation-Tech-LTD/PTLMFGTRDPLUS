using Microsoft.Extensions.DependencyInjection;
using PTLMFGPLUS.LIB.ConstantInfo;
using PTLMFGPLUS.LIB.ConstantInfo.IConstantInfo;
using PTLMFGPLUS.LIB.Repository;
using PTLMFGPLUS.LIB.Repository.IRepository;

namespace PTLMFGPLUS.LIB
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IModulePageProvider, InventoryPageProvider>();
            return services;
        }
    }
}
