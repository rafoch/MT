using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MT.Core.Context;
using MT.Core.Extensions;
using MT.Core.Model;

namespace MT.Core.Services
{
    public class TenantManager : TenantManager<Tenant, string>
    {
        public TenantManager(TenantCatalogContext<Tenant, string> context) : base(context)
        {
        }
    }

    public class TenantManager<TTenant> : TenantManager<TTenant, string>
        where TTenant : Tenant<string>
    {
        public TenantManager(TenantCatalogContext<TTenant, string> context) : base(context)
        {
        }
    }

    public class TenantManager<TTenant, TKey>
        where TTenant : Tenant<TKey> 
        where TKey : IEquatable<TKey>
    {
        private readonly TenantCatalogContext<TTenant, TKey> _context;

        public TenantManager(TenantCatalogContext<TTenant, TKey> context)
        {
            _context = context;
        }

        public void AddTenant(TTenant tenant)
        {
            _context.Tenants.Add(tenant);
            _context.SaveChanges();
        }

        public void AddTenantAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken))
        {
            _context.Tenants.AddAsync(tenant, cancellationToken);
            _context.SaveChangesAsync(cancellationToken);
        }

        public void RemoveTenant(TKey id)
        {
            var tenant = _context.Tenants.Filter(t => t.Id, id).FirstOrDefault();
            if (tenant is null)
            {
                throw new Exception();//todo our exceptions
            }
            RemoveTenant(tenant);
        }

        public void RemoveTenant(TTenant tenant)
        {
            _context.Tenants.Remove(tenant);
            _context.SaveChanges();
        }

        public TTenant Get(TKey id) => _context.Tenants.Filter(t => t.Id, id).FirstOrDefault();
    }
}