using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MT.Core.Model;

namespace MT.Core.Context
{
    public class TenantCatalogContext : TenantCatalogContext<Tenant, string>
    {
        public TenantCatalogContext(DbContextOptions options) : base(options)
        {
        }
    }

    public class TenantCatalogContext<TUser, TKey> : DbContext
        where TUser : Tenant<TKey>
        where TKey : IEquatable<TKey>
    {
        public TenantCatalogContext(DbContextOptions options)
            :base(options)
        {

        }

        public DbSet<TUser> Tenants { get; set; }
    }

}