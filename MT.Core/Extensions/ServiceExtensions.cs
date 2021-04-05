using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MT.Core.Context;
using MT.Core.Model;

namespace MT.Core.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMultiTenancyCatalog(this IServiceCollection service,
            Action<DbContextOptionsBuilder> options)
        {
            return service.AddMultiTenancyCatalog<TenantCatalogContext>(options);
        }

        public static IServiceCollection AddMultiTenancyCatalog<TUser>(
            this IServiceCollection service, Action<DbContextOptionsBuilder> options) 
            where TUser : DbContext
        {
            // Register Types

            // Add Context 
            service.AddDbContext<TUser>(options);
            return service;
        }

        public static IServiceCollection AddMultiTenancyContext<TUser>(
            this IServiceCollection service, Action<DbContextOptionsBuilder> options)
            where TUser : TenantContext
        {
            service.AddDbContext<TUser>(options);
            return service;
        }
    }
}