using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MT.Core.Context;
using MT.Core.Providers;
using MT.Core.Services;

namespace MT.Core.Model
{
    public class MultiTenancyBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="MultiTenancyBuilder"/>.
        /// </summary>
        /// <param name="tTenantType">The <see cref="Type"/> to use for the tTenantType catalog.</param>
        /// <param name="services">The <see cref="IServiceCollection"/> to attach to.</param>
        public MultiTenancyBuilder(Type tTenantType, IServiceCollection services)
        {
            TTenantTypeType = tTenantType;
            Services = services;
        }

        public MultiTenancyBuilder(Type tTenantType, Type tKeyType, IServiceCollection services) 
            : this(tTenantType, services)
        {
            this.TKeyType = tKeyType;
            AddTenantProvider();
        }

        public MultiTenancyBuilder(Type tTenantType, Type tKeyType, Type tTenancyType, IServiceCollection services) :
            this(tTenantType, tKeyType, services)
        {
            ITenancyType = tTenancyType;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> used catalog.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> used for catalog.
        /// </value>
        private Type TTenantTypeType { get; set; }


        /// <summary>
        /// Gets the <see cref="Type"/> used for tTenantType id type.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> used for tTenantType id type.
        /// </value>
        private Type TKeyType { get; set; }

        /// <summary>
        /// Gets the <see cref="Type"/> used for tTenantType type.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> used for tTenantType type.
        /// </value>
        private Type ITenancyType { get; set; }

        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> services are attached to.
        /// </summary>
        /// <value>
        /// The <see cref="IServiceCollection"/> services are attached to.
        /// </value>
        private IServiceCollection Services { get; set; }

        public virtual MultiTenancyBuilder AddTenantManager<TTenantManager>() where TTenantManager : class
        {
            var userManagerType = typeof(TenantManager<>).MakeGenericType(TTenantTypeType);
            var customType = typeof(TTenantManager);

            return AddScoped(userManagerType, customType);
        }

        /// <summary>
        /// Creates an new instance of Tenant Catalog Context where the informations about tenants are stored.
        /// </summary>
        /// <typeparam name="TTenantCatalogContext"></typeparam>
        /// <param name="optionsAction">database options builder</param>
        /// <returns><see cref="MultiTenancyBuilder"/></returns>
        public virtual MultiTenancyBuilder AddTenantCatalogContext<TTenantCatalogContext>(Action<DbContextOptionsBuilder> optionsAction)
            where TTenantCatalogContext : DbContext
        {
            AddDbContextOptionsBuilder<TTenantCatalogContext>((provider, builder) => optionsAction(builder));
            var userManagerType = typeof(TenantCatalogContext<,>).MakeGenericType(TTenantTypeType, TKeyType);
            var customType = typeof(TTenantCatalogContext);
            Services.AddDbContext<TTenantCatalogContext>(optionsAction);
            AddTenantManager();
            return AddScoped(userManagerType, customType);
        }

        /// <summary>
        /// Register in IoC <see cref="TTenantContext"/> with <see cref="ITenantContextFactory{TContext}"/>
        /// </summary>
        /// <typeparam name="TTenantContext">DbContext that inherits from <see cref="TenantContext"/></typeparam>
        /// <param name="optiAction"><see cref="DbContextOptionsBuilder"/></param>
        /// <returns><see cref="MultiTenancyBuilder"/></returns>
        public virtual MultiTenancyBuilder AddTenantContext<TTenantContext>(Action<DbContextOptionsBuilder> optiAction)
            where TTenantContext : DbContext
        {
            AddDbContextOptionsBuilder<TTenantContext>((provider, builder) => optiAction(builder));
            var userManagerType = typeof(TenantContext<,>).MakeGenericType(TTenantTypeType, TKeyType);
            var customType = typeof(TTenantContext);
            Services.AddDbContext<TTenantContext>(optiAction);
            AddTenantFactory(userManagerType, customType);
            return AddScoped(userManagerType, customType);
        }

        private MultiTenancyBuilder AddTenantFactory(Type userManagerType, Type type)
        {
            var makeGenericType = typeof(TenantContextFactory<,,,>).MakeGenericType(type, TTenantTypeType, ITenancyType, TKeyType);
            var customType = typeof(ITenantContextFactory<>).MakeGenericType(type);
            return AddScoped(customType, makeGenericType);
        }

        private MultiTenancyBuilder AddTenantProvider()
        {
            var userManagerType = typeof(TenantProvider<,>).MakeGenericType(TTenantTypeType, TKeyType);
            var serviceType = typeof(ITenantProvider<,>).MakeGenericType(TTenantTypeType, TKeyType);
            Services.AddScoped(serviceType, userManagerType);
            return this;
        }

        private MultiTenancyBuilder AddTenantManager()
        {
            var userManagerType = typeof(TenantManager<,>).MakeGenericType(TTenantTypeType, TKeyType);
            Services.AddScoped(userManagerType);
            return this;
        }

        private MultiTenancyBuilder AddScoped(Type serviceType, Type concreteType)
        {
            Services.AddScoped(serviceType, concreteType);
            return this;
        }

        private MultiTenancyBuilder AddDbContextOptionsBuilder<TContext>(
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction) 
            where TContext : DbContext
        {
            Services.TryAdd(new ServiceDescriptor(
                typeof(DbContextOptions<TContext>),
                p => CreateDbContextOptions<TContext>(p, optionsAction),
                ServiceLifetime.Scoped));
            
            Services.Add(
                new ServiceDescriptor(
                    typeof(DbContextOptions),
                    p => p.GetRequiredService<DbContextOptions<TContext>>(),
                    ServiceLifetime.Scoped));
            return this;
        }

        private static DbContextOptions<TContext> CreateDbContextOptions<TContext>(
            IServiceProvider applicationServiceProvider,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction)
            where TContext : DbContext
        {
            var builder = new DbContextOptionsBuilder<TContext>(
                new DbContextOptions<TContext>(new Dictionary<Type, IDbContextOptionsExtension>()));

            builder.UseApplicationServiceProvider(applicationServiceProvider);

            optionsAction?.Invoke(applicationServiceProvider, builder);

            return builder.Options;
        }
    }
}