using System;
using System.Diagnostics.CodeAnalysis;
using MT.Core.Exceptions;
using MT.Core.Model;

namespace MT.Core.Providers
{
    public interface ITenantProvider<TTenant, TKey>
        where TTenant : ITenancy<TKey>
        where TKey : IEquatable<TKey>
    {
        TKey Get();
        void Set(TKey key);
    }

    public class TenantProvider<TTenant, TKey> : ITenantProvider<TTenant, TKey>
        where TTenant : ITenancy<TKey>
        where TKey : IEquatable<TKey>
    {
        private TKey _tenantKey;

        /// <summary>
        /// Gets tenant id 
        /// </summary>
        /// <returns>tenant id</returns>
        public TKey Get()
        {
            if (_tenantKey is null)
            {
                throw new TenantNotProvidedException();
            }
            return _tenantKey;
        }

        /// <summary>
        /// Set tenant id
        /// </summary>
        /// <param name="key">tenant id</param>
        public void Set([NotNull] TKey key)
        {
            _tenantKey = key;
        }
    }
}