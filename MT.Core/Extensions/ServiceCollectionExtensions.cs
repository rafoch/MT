using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MT.Core.Context;
using MT.Core.Model;
using MT.Core.Services;

namespace MT.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMultiTenancyCatalog(this IServiceCollection service,
            Action<DbContextOptionsBuilder> options)
        {
            service.AddMultiTenancyCatalog<TenantCatalogContext>(options);
            return service;
        }

        public static IServiceCollection AddMultiTenancyCatalog<TContext>(
            this IServiceCollection service, Action<DbContextOptionsBuilder> options) 
            where TContext : DbContext
        {
            service.AddMultiTenancyCatalog<TContext, string>(options);
            return service;
        }

        public static IServiceCollection AddMultiTenancyCatalog<TContext, TKey>(
            this IServiceCollection service, Action<DbContextOptionsBuilder> options)
            where TContext : DbContext 
            where TKey : IEquatable<TKey>
        {
            //register types
            service.AddDbContext<TContext>();
            service.AddTransient<TContext>();
            // service.AddTransient<TenantManager<Tenant<TKey>, TKey>>();
            return service;
        }

        public static IServiceCollection AddMultiTenancyContext<TContext>(
            this IServiceCollection service, 
            Action<DbContextOptionsBuilder> options)
        where TContext : DbContext
        {
            service.AddDbContext<TContext>(options);
            return service;
        }
    }
}