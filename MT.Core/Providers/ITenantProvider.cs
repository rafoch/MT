using System;
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
            return _tenantKey;
        }

        public void Set(TKey key)
        {
            _tenantKey = key;
        }
    }
}