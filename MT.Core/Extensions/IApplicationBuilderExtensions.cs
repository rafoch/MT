using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MT.Core.Context;
using MT.Core.Middlewares;
using MT.Core.Model;
using MT.Core.Providers;

namespace MT.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IApplicationBuilder"/>
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds <see cref="TenantHttpMiddleware{TTenant,TKey}"/> to application
        /// </summary>
        /// <typeparam name="TTenant"><see cref="Tenant{TKey}"/></typeparam>
        /// <typeparam name="TKey"><see cref="Tenant{TKey}.Id"/></typeparam>
        /// <param name="builder"><see cref="IApplicationBuilder"/></param>
        public static void UseMultiTenancy<TTenant, TKey>(this IApplicationBuilder builder)
        where TTenant : Tenant<TKey> 
        where TKey : IEquatable<TKey>
        {
            builder.UseMiddleware<TenantHttpMiddleware<TTenant,TKey>>();
        }

        /// <summary>
        /// Migrates every tenant database stored in <see cref="TenantCatalogDbContext{TTenant,TKey}.Tenants"/> table on application startup
        /// </summary>
        /// <typeparam name="TTenant"><see cref="Tenant{TKey}"/></typeparam>
        /// <typeparam name="TKey"><see cref="Tenant{TKey}.Id"/></typeparam>
        /// <param name="builder"><see cref="IApplicationBuilder"/></param>
        public static void MigrateTenantDatabases<TTenant, TKey>(this IApplicationBuilder builder)
            where TTenant : Tenant<TKey>
            where TKey : IEquatable<TKey>
        {
            List<TKey> keys = null;
            using (var serviceScope = builder.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
            {
                var tenantCatalogContext = serviceScope?.ServiceProvider.GetService<TenantCatalogDbContext<TTenant, TKey>>();
                if (tenantCatalogContext != null)
                {
                    keys = tenantCatalogContext.Tenants.Select(tenant => tenant.Id).ToList();
                }
            }

            if (keys == null)
            {
                return;
            }

            foreach (var key in keys)
            {
                MigrateTenantContext<TTenant, TKey>(builder, key);
            }
        }

        /// <summary>
        /// Creates dbContext to tenant database and migrate it during application startup 
        /// </summary>
        /// <typeparam name="TTenant"><see cref="Tenant{TKey}"/></typeparam>
        /// <typeparam name="TKey"><see cref="Tenant{TKey}.Id"/></typeparam>
        /// <param name="app"><see cref="IApplicationBuilder"/></param>
        /// <param name="key"><see cref="Tenant{TKey}.Id"/></param>
        private static void MigrateTenantContext<TTenant, TKey>(
            IApplicationBuilder app, 
            TKey key) 
            where TTenant : Tenant<TKey>
            where TKey : IEquatable<TKey>
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
            {
                if (serviceScope != null)
                {
                    var provider = serviceScope.ServiceProvider.GetService<ITenantProvider<TTenant, TKey>>();
                    provider?.Set(key);
                }

                var enumerable = serviceScope?.ServiceProvider.GetServices(typeof(TenantDbContext<TTenant, TKey>));
                if (enumerable != null)
                    foreach (var tenancyContext in enumerable)
                    {
                        var makeGenericType = typeof(ITenantDbContextFactory<>).MakeGenericType(tenancyContext?.GetType()!);
                        var service = GetService<TTenant, TKey>(serviceScope, makeGenericType) as ITenantDbContextFactory<DbContext>;

                        var database = service?.Create().Database;
                        if (database != null && 
                            database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory" && 
                            database.CanConnect())
                        {
                            // database.Migrate();
                        }
                    }
            }
        }

        /// <summary>
        /// Gets service by type
        /// </summary>
        /// <typeparam name="TTenant"><see cref="Tenant{TKey}"/></typeparam>
        /// <typeparam name="TKey"><see cref="Tenant{TKey}.Id"/></typeparam>
        /// <param name="serviceScope"><see cref="IServiceScope"/></param>
        /// <param name="serviceType"><see cref="Type"/></param>
        /// <returns></returns>
        private static object GetService<TTenant, TKey>(IServiceScope serviceScope, Type serviceType) 
            where TTenant : Tenant<TKey> 
            where TKey : IEquatable<TKey>
        {
            return serviceScope.ServiceProvider.GetService(serviceType);
        }
    }
}