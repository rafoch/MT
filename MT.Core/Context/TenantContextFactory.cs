using System;
using System.Data.SqlClient;
using MT.Core.Exceptions;
using MT.Core.Model;
using MT.Core.Providers;
using MT.Core.Services;

namespace MT.Core.Context
{
    public class TenantContextFactory<TContext, TTenant, TTenancy,TKey> : ITenantContextFactory<TContext>
        where TContext : TenantContext<TTenant, TKey> 
        where TTenancy : ITenancy<TKey> 
        where TKey : IEquatable<TKey>
        where TTenant : Tenant<TKey>
    {
        private readonly ITenantProvider<TTenant, TKey> _tenantProvider;
        private readonly TenantManager<TTenant, TKey> _tenantManager;
        private TContext _context;
        
        public TenantContextFactory(
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
           
            var password = _tenantManager.GetTenantPassword(tenant.Password, tenant.ConcurencyStamp);
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
    public interface ITenantContextFactory<out TContext> : IDisposable
    {
        /// <summary>
        /// Creates delivered <see cref="TenantContext"/>
        /// </summary>
        /// <returns>Return context that inherits from <see cref="TenantContext"/></returns>
        TContext Create();
    }
}