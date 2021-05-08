using System;
using System.Data.SqlClient;
using MT.Core.Exceptions;
using MT.Core.Model;
using MT.Core.Providers;
using MT.Core.Services;

namespace MT.Core.Context
{
    /// <inheritdoc />
    public class TenantDbContextFactory<TContext, TTenant, TTenancy,TKey> : ITenantDbContextFactory<TContext>
        where TContext : TenantDbContext<TTenant, TKey> 
        where TTenancy : Tenancy<TKey> 
        where TKey : IEquatable<TKey>
        where TTenant : Tenant<TKey>
    {
        private readonly ITenantProvider<TTenant, TKey> _tenantProvider;
        private readonly TenantManager<TTenant, TKey> _tenantManager;
        private TContext _context;
        
        /// <summary>
        /// Creates new instance of <see cref="TenantDbContextFactory{TContext,TTenant,TTenancy,TKey}"/>
        /// </summary>
        /// <param name="tenantProvider"><see cref="ITenantProvider{TTenant,TKey}"/></param>
        /// <param name="tenantManager"><see cref="TenantManager{TTenant}"/></param>
        public TenantDbContextFactory(
            ITenantProvider<TTenant, TKey> tenantProvider,
            TenantManager<TTenant, TKey> tenantManager)
        {
            _tenantProvider = tenantProvider;
            _tenantManager = tenantManager;
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

            _context = Activator.CreateInstance(typeof(TContext), connectionStringBuilder, _tenantProvider) as TContext;
            return _context;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _context?.Dispose();
        }
    }

    /// <inheritdoc />
    public interface ITenantDbContextFactory<out TContext> : IDisposable
    {
        /// <summary>
        /// Creates delivered <see cref="TenantDbContext"/>
        /// </summary>
        /// <returns>Return dbContext that inherits from <see cref="TenantDbContext"/></returns>
        TContext Create();
    }
}