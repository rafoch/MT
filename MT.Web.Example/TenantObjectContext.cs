using System;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MT.Core.Context;
using MT.Core.Providers;

namespace MT.Web.Example
{
    public class TenantDbObjectContext : TenantDbContext<TenantCatalog, Guid>
    {
        public TenantDbObjectContext(
            ITenantProvider<TenantCatalog, Guid> provider,
            DbContextOptions<TenantDbObjectContext> options) : base(provider, options)
        {
        }

        public TenantDbObjectContext(
            SqlConnectionStringBuilder connectionStringBuilder, 
            ITenantProvider<TenantCatalog, Guid> provider) 
            : base(connectionStringBuilder, provider)
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

        public TenantDbObjectTwoContext(ITenantProvider<TenantCatalog, Guid> provider, DbContextOptions options) : base(provider, options)
        {
        }

        public TenantDbObjectTwoContext(SqlConnectionStringBuilder connectionStringBuilder, ITenantProvider<TenantCatalog, Guid> provider) : base(connectionStringBuilder, provider)
        {
        }
    }
}