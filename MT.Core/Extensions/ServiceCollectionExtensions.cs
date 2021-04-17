using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MT.Core.Model;

namespace MT.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static MultiTenancyBuilder AddMultiTenancy(this IServiceCollection service)
        {
            return AddMultiTenancy<Tenant>(service);
        }

        public static MultiTenancyBuilder AddMultiTenancy<TTenant>(
            this IServiceCollection service)
            where TTenant : Tenant<string>
        {
            return new MultiTenancyBuilder(typeof(TTenant), typeof(string), service);
        }

        public static MultiTenancyBuilder AddMultiTenancy<TTenant, TKey>(
            this IServiceCollection service)
            where TTenant : Tenant<TKey>
            where TKey : IEquatable<TKey>
        {
            return new MultiTenancyBuilder(typeof(TTenant), typeof(TKey), service);
        }

        public static MultiTenancyBuilder AddMultiTenancy<TTenant, TKey, ITenancy>(
            this IServiceCollection service)
            where TTenant : Tenant<TKey>
            where TKey : IEquatable<TKey>
        {
            return new MultiTenancyBuilder(typeof(TTenant), typeof(TKey), typeof(ITenancy), service);
        }
        
        public static IServiceCollection AddMultiTenancyContext<TContext>(
            this IServiceCollection service,
            Action<DbContextOptionsBuilder> options)
        where TContext : DbContext
        {
            service.AddDbContext<TContext>(options);

            return service;
        }

        private static void AddScoped(IServiceCollection service, Type serviceType, Type concreteType)
        {
            service.AddScoped(serviceType, concreteType);
        }
    }
}