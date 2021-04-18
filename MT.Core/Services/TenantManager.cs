using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MT.Core.Context;
using MT.Core.Exceptions;
using MT.Core.Extensions;
using MT.Core.Model;

namespace MT.Core.Services
{
    public class TenantManager : TenantManager<Tenant, string>
    {
        public TenantManager(TenantCatalogContext<Tenant, string> context,
            IDataProtectionProvider dataProtectionProvider) : base(context, dataProtectionProvider)
        {
        }
    }

    public class TenantManager<TTenant> : TenantManager<TTenant, string>
        where TTenant : Tenant<string>
    {
        public TenantManager(TenantCatalogContext<TTenant, string> context,
            IDataProtectionProvider dataProtectionProvider) : base(context, dataProtectionProvider)
        {
        }
    }

    public class TenantManager<TTenant, TKey>
        where TTenant : Tenant<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly TenantCatalogContext<TTenant, TKey> _context;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public TenantManager(
            TenantCatalogContext<TTenant, TKey> context,
            IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
            _dataProtectionProvider = dataProtectionProvider;
        }

        public TTenant AddTenant(TTenant tenant)
        {
            _context.Tenants.Add(tenant);
            _context.SaveChanges();
            return tenant;
        }

        public async Task<TTenant> AddTenantAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dataProtector = _dataProtectionProvider.CreateProtector("password hash");
            var protect = dataProtector.Protect(tenant.Password);
            tenant.Password = protect;
            await _context.Tenants.AddAsync(tenant, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return tenant;
        }

        public void RemoveTenant(TKey id)
        {
            var tenant = _context.Tenants.Filter(t => t.Id, id).FirstOrDefault();
            if (tenant is null)
            {
                throw new TenantNotFoundException(id.ToString());
            }
            RemoveTenant(tenant);
        }

        public void RemoveTenant(TTenant tenant)
        {
            _context.Tenants.Remove(tenant);
            _context.SaveChanges();
        }

        public async Task RemoveTenantAsync(TKey id)
        {
            var tenant = await _context.Tenants.Filter(t => t.Id, id).FirstOrDefaultAsync();
            if (tenant is null)
            {
                throw new TenantNotFoundException(id.ToString());
            }
            await RemoveTenantAsync(tenant);
        }

        public async Task RemoveTenantAsync(TTenant tenant)
        {
            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();
        }

        public TTenant Get(TKey id) => _context.Tenants.Filter(t => t.Id, id).FirstOrDefault();
        public Task<TTenant> GetAsync(TKey id) => _context.Tenants.Filter(t => t.Id, id).FirstOrDefaultAsync();
    }
}