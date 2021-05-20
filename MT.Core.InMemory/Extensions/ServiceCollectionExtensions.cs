using MT.Core.InMemory.Factories;
using MT.Core.InMemory.Providers;
using MT.Core.Model;

namespace MT.Core.InMemory.Extensions
{
    /// <inheritdoc cref="ServiceCollectionExtensions"/>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register an <see cref="InMemoryOnConfiguringDbContextOptionsBuilderProvider"/> provider and <see cref="TenantDbContextFactory{TContext,TTenant,TTenancy,TKey}"/>
        /// for InMemory Database
        /// </summary>
        /// <param name="service"><see cref="MultiTenancyBuilder"/></param>
        /// <returns><see cref="MultiTenancyBuilder"/></returns>
        public static MultiTenancyBuilder UseInMemory(this MultiTenancyBuilder service)
        {
            service.RegisterOnConfiguringProvider(typeof(InMemoryOnConfiguringDbContextOptionsBuilderProvider));
            return service.AddTenantFactory(typeof(TenantDbContextFactory<,,,>));
        }
    }
}