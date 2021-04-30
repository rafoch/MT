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

        public TKey Get()
        {
            if (_tenantKey is null)
            {
                throw new TenantNotProvidedException();
            }
            return _tenantKey;
        }

        public void Set([NotNull] TKey key)
        {
            _tenantKey = key;
        }
    }
}