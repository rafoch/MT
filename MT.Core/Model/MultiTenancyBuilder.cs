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
    /// <summary>
    /// Builder that provides various method to use Multitenancy in your application 
    /// </summary>
    public class MultiTenancyBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="MultiTenancyBuilder"/>.
        /// </summary>
        /// <param name="tenantType">The <see cref="Type"/> to use for the tenantType catalog.</param>
        /// <param name="services">The <see cref="IServiceCollection"/> to attach to.</param>
        public MultiTenancyBuilder(Type tenantType, IServiceCollection services)
        {
            TenantType = tenantType;
            Services = services;
        }

        /// <inheritdoc />
        public MultiTenancyBuilder(Type tenantType, Type tKeyType, IServiceCollection services) 
            : this(tenantType, services)
        {
            this.KeyType = tKeyType;
            AddTenantProvider();
        }

        /// <inheritdoc />
        public MultiTenancyBuilder(Type tenantType, Type tKeyType, Type tTenancyType, IServiceCollection services) :
            this(tenantType, tKeyType, services)
        {
            TenancyType = tTenancyType;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> used catalog.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> used for catalog.
        /// </value>
        private Type TenantType { get; set; }


        /// <summary>
        /// Gets the <see cref="Type"/> used for <see cref="Tenant{TKey}.Id"/>.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> used for <see cref="Tenant{TKey}.Id"/>.
        /// </value>
        private Type KeyType { get; set; }

        /// <summary>
        /// Gets the <see cref="Type"/> used for tenantType type.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> used for tenantType type.
        /// </value>
        private Type TenancyType { get; set; }

        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> services are attached to.
        /// </summary>
        /// <value>
        /// The <see cref="IServiceCollection"/> services are attached to.
        /// </value>
        private IServiceCollection Services { get; set; }

        /// <summary>
        /// Register TenantManager in <see cref="IServiceCollection"/>
        /// </summary>
        /// <typeparam name="TTenantManager"><see cref="TenantManager{TTenant}"/></typeparam>
        /// <returns><see cref="MultiTenancyBuilder"/></returns>
        public virtual MultiTenancyBuilder AddTenantManager<TTenantManager>() where TTenantManager : class
        {
            var userManagerType = typeof(TenantManager<>).MakeGenericType(TenantType);
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
            var userManagerType = typeof(TenantCatalogDbContext<,>).MakeGenericType(TenantType, KeyType);
            var customType = typeof(TTenantCatalogContext);
            Services.AddDbContext<TTenantCatalogContext>(optionsAction);
            AddTenantManager();
            return AddScoped(userManagerType, customType);
        }

        /// <summary>
        /// Register provided <see cref="TenantDbContext{TTenant,TKey}"/> in <see cref="IServiceCollection"/>
        /// </summary>
        /// <typeparam name="TTenantContext">DbContext that inherits from <see cref="TenantDbContext"/></typeparam>
        /// <param name="optiAction"><see cref="DbContextOptionsBuilder"/></param>
        /// <returns><see cref="MultiTenancyBuilder"/></returns>
        public virtual MultiTenancyBuilder AddTenantContext<TTenantContext>(Action<DbContextOptionsBuilder> optiAction)
            where TTenantContext : DbContext
        {
            AddDbContextOptionsBuilder<TTenantContext>((provider, builder) => optiAction(builder));
            var userManagerType = typeof(TenantDbContext<,>).MakeGenericType(TenantType, KeyType);
            var customType = typeof(TTenantContext);
            Services.AddDbContext<TTenantContext>(optiAction);
            AddTenantFactory(userManagerType, customType);
            return AddScoped(userManagerType, customType);
        }

        private MultiTenancyBuilder AddTenantFactory(Type userManagerType, Type type)
        {
            var makeGenericType = typeof(TenantDbContextFactory<,,,>).MakeGenericType(type, TenantType, TenancyType, KeyType);
            var customType = typeof(ITenantDbContextFactory<>).MakeGenericType(type);
            return AddScoped(customType, makeGenericType);
        }

        private MultiTenancyBuilder AddTenantProvider()
        {
            var userManagerType = typeof(TenantProvider<,>).MakeGenericType(TenantType, KeyType);
            var serviceType = typeof(ITenantProvider<,>).MakeGenericType(TenantType, KeyType);
            Services.AddScoped(serviceType, userManagerType);
            return this;
        }

        private MultiTenancyBuilder AddTenantManager()
        {
            var userManagerType = typeof(TenantManager<,>).MakeGenericType(TenantType, KeyType);
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