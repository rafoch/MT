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
    public static class IApplicationBuilderExtensions
    {
        public static void UseMultiTenancy<TTenant, TKey>(this IApplicationBuilder builder)
        where TTenant : Tenant<TKey> 
        where TKey : IEquatable<TKey>
        {
            builder.UseMiddleware<TenantHttpMiddleware<TTenant,TKey>>();
        }

        public static void MigrateTenantDatabases<TTenant, TKey>(this IApplicationBuilder app)
            where TTenant : Tenant<TKey>
            where TKey : IEquatable<TKey>
        {
            List<TKey> keys;
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var tenantCatalogContext = serviceScope.ServiceProvider.GetService<TenantCatalogContext<TTenant, TKey>>();
                keys = tenantCatalogContext.Tenants.Select(tenant => tenant.Id).ToList();
            }

            foreach (var key in keys)
            {
                MigrateTenantContext<TTenant, TKey>(app, key);
            }
        }

        private static void MigrateTenantContext<TTenant, TKey>(
            IApplicationBuilder app, 
            TKey key) 
            where TTenant : Tenant<TKey>
            where TKey : IEquatable<TKey>
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var provider = serviceScope.ServiceProvider.GetService<ITenantProvider<TTenant, TKey>>();
                provider.Set(key);
                var enumerable = serviceScope.ServiceProvider.GetServices(typeof(TenantContext<TTenant, TKey>));
                foreach (var tenancyContext in enumerable)
                {
                    var makeGenericType = typeof(ITenantContextFactory<>).MakeGenericType(tenancyContext.GetType());
                    var service =
                        GetService<TTenant, TKey>(serviceScope, makeGenericType) as ITenantContextFactory<DbContext>;

                    var database = service.Create().Database;
                    if (database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory" && database.CanConnect())
                    {
                        database.Migrate();
                    }
                }
            }
        }

        private static object GetService<TTenant, TKey>(IServiceScope serviceScope, Type makeGenericType) 
            where TTenant : Tenant<TKey> 
            where TKey : IEquatable<TKey>
        {
            return serviceScope.ServiceProvider.GetService(makeGenericType);
        }
    }
}