using System;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MT.Core.Context;
using MT.Core.Providers;

namespace MT.Web.Example
{
    public class TenantObjectContext : TenantContext<TenantCatalog, Guid>
    {
        public TenantObjectContext(ITenantProvider<TenantCatalog, Guid> provider,
            DbContextOptions<TenantObjectContext> options) : base(provider, options)
        {
        }

        public TenantObjectContext(
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

    public class TenantObjectTwoContext : TenantContext<TenantCatalog, Guid>
    {
        public DbSet<TenantObjectTwo> TenantObjectTwos { get; set; }

        public TenantObjectTwoContext(ITenantProvider<TenantCatalog, Guid> provider, DbContextOptions options) : base(provider, options)
        {
        }

        public TenantObjectTwoContext(SqlConnectionStringBuilder connectionStringBuilder, ITenantProvider<TenantCatalog, Guid> provider) : base(connectionStringBuilder, provider)
        {
        }
    }
}