using Microsoft.Extensions.DependencyInjection;
using MT.Core.Model;
using MT.Core.SqlServer.Factories;
using MT.Core.SqlServer.Providers;

namespace MT.Core.SqlServer.Extensions
{
    /// <summary>
    /// <see cref="ServiceCollectionExtensions"/>
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register an <see cref="SqlServerOnConfiguringDbContextOptionsBuilderProvider"/> provider and <see cref="TenantDbContextFactory{TContext,TTenant,TTenancy,TKey}"/>
        /// </summary>
        /// <param name="service"><see cref="MultiTenancyBuilder"/></param>
        /// <returns><see cref="MultiTenancyBuilder"/></returns>
        public static MultiTenancyBuilder UseSqlServer(this MultiTenancyBuilder service)
        {
            service.RegisterOnConfiguringProvider(typeof(SqlServerOnConfiguringDbContextOptionsBuilderProvider));
            return service.AddTenantFactory(typeof(TenantDbContextFactory<,,,>));
        }
    }
}