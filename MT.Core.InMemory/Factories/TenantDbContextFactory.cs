using System;
using System.Data.SqlClient;
using MT.Core.Context;
using MT.Core.Exceptions;
using MT.Core.Interfaces;
using MT.Core.Model;
using MT.Core.Providers;
using MT.Core.Services;

namespace MT.Core.InMemory.Factories
{
    /// <inheritdoc />
    public class TenantDbContextFactory<TContext, TTenant, TTenancy, TKey> : Context.ITenantDbContextFactory<TContext>
        where TContext : TenantDbContext<TTenant, TKey>
        where TTenancy : Tenancy<TKey>
        where TKey : IEquatable<TKey>
        where TTenant : Tenant<TKey>
    {
        private readonly ITenantProvider<TTenant, TKey> _tenantProvider;
        private readonly TenantManager<TTenant, TKey> _tenantManager;
        private readonly IOnConfiguringDbContextOptionsBuilderProvider _onConfiguringDbContextOptionsBuilderProvider;
        private TContext _context;

        /// <summary>
        /// Creates new instance of <see cref="TenantDbContextFactory{TContext,TTenant,TTenancy,TKey}"/>
        /// </summary>
        /// <param name="tenantProvider"><see cref="ITenantProvider{TTenant,TKey}"/></param>
        /// <param name="tenantManager"><see cref="TenantManager{TTenant}"/></param>
        /// <param name="onConfiguringDbContextOptionsBuilderProvider"><see cref="IOnConfiguringDbContextOptionsBuilderProvider"/></param>
        public TenantDbContextFactory(
            ITenantProvider<TTenant, TKey> tenantProvider,
            TenantManager<TTenant, TKey> tenantManager,
            IOnConfiguringDbContextOptionsBuilderProvider onConfiguringDbContextOptionsBuilderProvider)
        {
            _tenantProvider = tenantProvider;
            _tenantManager = tenantManager;
            _onConfiguringDbContextOptionsBuilderProvider = onConfiguringDbContextOptionsBuilderProvider;
        }

        /// <inheritdoc />
        public TContext Create()
        {
            var value = _tenantProvider.Get().ToString();
            if (string.IsNullOrEmpty(value))
            {
                throw new TenantNotProvidedException();
            }
            var tenant = _tenantManager.Get(_tenantProvider.Get());
            if (tenant is null)
            {
                throw new TenantNotFoundException(_tenantProvider.Get().ToString());
            }

            var password = _tenantManager.GetTenantPassword(tenant.Password, tenant.ConcurrencyStamp);
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = $"tcp:{tenant.Server},{tenant.Port}",
                InitialCatalog = tenant.Database,
                UserID = tenant.UserName,
                Password = password,
            };

            _context = Activator.CreateInstance(typeof(TContext), connectionStringBuilder, _tenantProvider, _onConfiguringDbContextOptionsBuilderProvider) as TContext;
            return _context;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}