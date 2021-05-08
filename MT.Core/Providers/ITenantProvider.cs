using System;
using System.Diagnostics.CodeAnalysis;
using MT.Core.Exceptions;
using MT.Core.Model;

namespace MT.Core.Providers
{
    /// <inheritdoc />
    public interface ITenantProvider : ITenantProvider<Tenant, string>
    {
    }

    /// <summary>
    /// Creates new instance of <see cref="ITenantProvider{TTenant,TKey}"/>
    /// </summary>
    /// <typeparam name="TTenant"><see cref="Tenant{TKey}"/></typeparam>
    /// <typeparam name="TKey"><see cref="Tenant{TKey}.Id"/></typeparam>
    public interface ITenantProvider<TTenant, TKey>
        where TTenant : Tenant<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Method returns <see cref="Tenant{TKey}.Id"/> stored in memory
        /// </summary>
        /// <returns><see cref="Tenant{TKey}.Id"/></returns>
        TKey Get();

        /// <summary>
        /// Sets <see cref="Tenant{TKey}.Id"/> value in memory
        /// </summary>
        /// <param name="key"><see cref="Tenant{TKey}.Id"/></param>
        void Set(TKey key);
    }

    /// <inheritdoc />
    public class TenantProvider<TTenant, TKey> : ITenantProvider<TTenant, TKey>
        where TTenant : Tenant<TKey>
        where TKey : IEquatable<TKey>
    {
        private TKey _tenantKey;

        /// <summary>
        /// Method returns <see cref="Tenant{TKey}.Id"/> stored in memory
        /// </summary>
        /// <returns><see cref="Tenant{TKey}.Id"/></returns>
        public TKey Get()
        {
            if (_tenantKey is null)
            {
                throw new TenantNotProvidedException();
            }
            return _tenantKey;
        }

        /// <summary>
        /// Sets <see cref="Tenant{TKey}.Id"/> value in memory
        /// </summary>
        /// <param name="key"><see cref="Tenant{TKey}.Id"/></param>
        public void Set([NotNull] TKey key)
        {
            _tenantKey = key;
        }
    }
}