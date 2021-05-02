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
        /// <param name="tenantCatalog">The <see cref="Type"/> to use for the tenant catalog.</param>
        /// <param name="services">The <see cref="IServiceCollection"/> to attach to.</param>
        public MultiTenancyBuilder(Type tenantCatalog, IServiceCollection services)
        {
            TenantCatalogType = tenantCatalog;
            Services = services;
        }

        public MultiTenancyBuilder(Type tenantCatalog, Type idType, IServiceCollection services) : this(tenantCatalog, services)
            => this.IdType = idType;

        public MultiTenancyBuilder(Type tenantCatalog, Type idType, Type tenantType, IServiceCollection services) : this(tenantCatalog, idType, services)
            => TenantType = tenantType;

        /// <summary>
        /// Gets the <see cref="Type"/> used catalog.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> used for catalog.
        /// </value>
        private Type TenantCatalogType { get; set; }


        /// <summary>
        /// Gets the <see cref="Type"/> used for tenant id type.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> used for tenant id type.
        /// </value>
        private Type IdType { get; set; }

        /// <summary>
        /// Gets the <see cref="Type"/> used for tenant type.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> used for tenant type.
        /// </value>
        private Type TenantType { get; set; }

        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> services are attached to.
        /// </summary>
        /// <value>
        /// The <see cref="IServiceCollection"/> services are attached to.
        /// </value>
        private IServiceCollection Services { get; set; }

        public virtual MultiTenancyBuilder AddTenantManager<TTenantManager>() where TTenantManager : class
        {
            var userManagerType = typeof(TenantManager<>).MakeGenericType(TenantCatalogType);
            var customType = typeof(TTenantManager);

            return AddScoped(userManagerType, customType);
        }

        /// <summary>
        /// Creates an new instance of Tenant Catalog Context where the informations about tenants are stored.
        /// </summary>
        /// <typeparam name="TTenantCatalogContext"></typeparam>
        /// <param name="optionsAction">database options builder</param>
        /// <returns></returns>
        public virtual MultiTenancyBuilder AddTenantCatalogContext<TTenantCatalogContext>(Action<DbContextOptionsBuilder> optionsAction)
            where TTenantCatalogContext : DbContext
        {
            AddDbContextOptionsBuilder<TTenantCatalogContext>((provider, builder) => optionsAction(builder));
            var userManagerType = typeof(TenantCatalogContext<,>).MakeGenericType(TenantCatalogType, IdType);
            var customType = typeof(TTenantCatalogContext);
            Services.AddDbContext<TTenantCatalogContext>(optionsAction);
            AddTenantManager();
            return AddScoped(userManagerType, customType);
        }

        public virtual MultiTenancyBuilder AddTenantContext<TTenantContext>(Action<DbContextOptionsBuilder> optiAction)
            where TTenantContext : DbContext
        {
            AddDbContextOptionsBuilder<TTenantContext>((provider, builder) => optiAction(builder));
            var userManagerType = typeof(TenantContext<,>).MakeGenericType(TenantType, IdType);
            var customType = typeof(TTenantContext);
            AddTenantProvider();
            AddTenantFactory(userManagerType, customType);
            Services.AddDbContext<TTenantContext>(optiAction);
            return AddScoped(userManagerType, customType);
        }

        private MultiTenancyBuilder AddTenantFactory(Type userManagerType, Type type)
        {
            var makeGenericType = typeof(TenantContextFactory<,,,>).MakeGenericType(type, TenantCatalogType, TenantType, IdType);
            var customType = typeof(ITenantContextFactory<>).MakeGenericType(type);
            return AddScoped(customType, makeGenericType);
        }

        private MultiTenancyBuilder AddTenantProvider()
        {
            var userManagerType = typeof(TenantProvider<,>).MakeGenericType(TenantType, IdType);
            var serviceType = typeof(ITenantProvider<,>).MakeGenericType(TenantType, IdType);
            Services.AddScoped(serviceType, userManagerType);
            return this;
        }

        private MultiTenancyBuilder AddTenantManager()
        {
            var userManagerType = typeof(TenantManager<,>).MakeGenericType(TenantCatalogType, IdType);
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

        /// <summary>
        /// Migrate all tenants databases to the current migrations on application startup
        /// </summary>
        /// <returns><see cref="MultiTenancyBuilder"/> object</returns>
        public MultiTenancyBuilder MigrateTenantContexts()
        {
            return this;
        }
    }
}