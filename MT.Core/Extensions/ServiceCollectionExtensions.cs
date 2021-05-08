using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MT.Core.Context;
using MT.Core.Model;

namespace MT.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/>
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Multitenancy to your application
        /// </summary>
        /// <param name="service"><see cref="IServiceCollection"/></param>
        /// <returns><see cref="MultiTenancyBuilder"/></returns>
        public static MultiTenancyBuilder AddMultiTenancy(this IServiceCollection service)
        {
            return AddMultiTenancy<Tenant>(service);
        }

        /// <summary>
        /// Adds Multitenancy to your application 
        /// </summary>
        /// <param name="service"><see cref="IServiceCollection"/></param>
        /// <typeparam name="TTenant">Object that represents your tenant object and it inherits from <see cref="Tenant{TKey}"/></typeparam>
        /// <returns><see cref="MultiTenancyBuilder"/></returns>
        public static MultiTenancyBuilder AddMultiTenancy<TTenant>(
            this IServiceCollection service)
            where TTenant : Tenant<string>
        {
            return new MultiTenancyBuilder(typeof(TTenant), typeof(string), service);
        }

        /// <summary>
        /// Adds Multitenancy to your application 
        /// </summary>
        /// <param name="service"><see cref="IServiceCollection"/></param>
        /// <typeparam name="TTenant">Object that represents your tenant object and it inherits from <see cref="Tenant{TKey}"/></typeparam>
        /// <typeparam name="TKey"><see cref="Tenant{TKey}.Id"/></typeparam>
        /// <returns><see cref="MultiTenancyBuilder"/></returns>
        public static MultiTenancyBuilder AddMultiTenancy<TTenant, TKey>(
            this IServiceCollection service)
            where TTenant : Tenant<TKey>
            where TKey : IEquatable<TKey>
        {
            var tTenancyType = typeof(Tenancy<TKey>);
            return new MultiTenancyBuilder(typeof(TTenant), typeof(TKey), tTenancyType, service);
        }

        /// <summary>
        /// Adds Multitenancy to your application 
        /// </summary>
        /// <param name="service"><see cref="IServiceCollection"/></param>
        /// <typeparam name="TTenant">Object that represents your tenant object and it inherits from <see cref="Tenant{TKey}"/></typeparam>
        /// <typeparam name="TKey"><see cref="Tenant{TKey}.Id"/></typeparam>
        /// <typeparam name="TTenancy"><see cref="Tenancy{TKey}"/></typeparam>
        /// <returns><see cref="MultiTenancyBuilder"/></returns>
        public static MultiTenancyBuilder AddMultiTenancy<TTenant, TTenancy, TKey>(
            this IServiceCollection service)
            where TTenant : Tenant<TKey>
            where TKey : IEquatable<TKey>
        {
            return new MultiTenancyBuilder(typeof(TTenant), typeof(TKey), typeof(TTenancy), service);
        }
        
        private static void AddScoped(IServiceCollection service, Type serviceType, Type concreteType)
        {
            service.AddScoped(serviceType, concreteType);
        }
    }
}