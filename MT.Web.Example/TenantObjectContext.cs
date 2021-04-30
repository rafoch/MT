using System;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MT.Core.Context;
using MT.Core.Providers;

namespace MT.Web.Example
{
    public class TenantObjectContext : TenantContext<TenantObject, Guid>
    {
        public TenantObjectContext(ITenantProvider<TenantObject, Guid> provider,
            DbContextOptions<TenantObjectContext> options) : base(provider, options)
        {
        }

        public TenantObjectContext(
            SqlConnectionStringBuilder connectionStringBuilder, 
            ITenantProvider<TenantObject, Guid> provider) 
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
}