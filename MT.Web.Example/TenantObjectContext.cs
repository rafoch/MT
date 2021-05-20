using System;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MT.Core.Context;
using MT.Core.Interfaces;
using MT.Core.Providers;

namespace MT.Web.Example
{
    public class TenantDbObjectContext : TenantDbContext<TenantCatalog, Guid>
    {
        public TenantDbObjectContext(
            ITenantProvider<TenantCatalog, Guid> tenantProvider,
            DbContextOptions<TenantDbObjectContext> options) : base(tenantProvider, options)
        {
        }

        public TenantDbObjectContext(
            SqlConnectionStringBuilder connectionStringBuilder, 
            ITenantProvider<TenantCatalog, Guid> tenantProvider,
            IOnConfiguringDbContextOptionsBuilderProvider provider) 
            : base(connectionStringBuilder, tenantProvider, provider)
        {
        }

        protected TenantDbObjectContext(DbContextOptions connectionStringBuilder) : base(connectionStringBuilder)
        {
        }

        public DbSet<TenantObject> TenantObject { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TenantObject>();
            base.OnModelCreating(modelBuilder);
        }
    }

    public class TenantDbObjectTwoContext : TenantDbContext<TenantCatalog, Guid>
    {
        public DbSet<TenantObjectTwo> TenantObjectTwos { get; set; }

        public TenantDbObjectTwoContext(
            ITenantProvider<TenantCatalog, Guid> tenantProvider, 
            DbContextOptions options) : base(tenantProvider, options)
        {
        }

        public TenantDbObjectTwoContext(
            SqlConnectionStringBuilder connectionStringBuilder, 
            ITenantProvider<TenantCatalog, Guid> tenantProvider,
            IOnConfiguringDbContextOptionsBuilderProvider provider) : base(connectionStringBuilder, tenantProvider, provider)
        {
        }
    }
}