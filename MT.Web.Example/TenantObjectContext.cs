using Microsoft.EntityFrameworkCore;
using MT.Core.Context;
using MT.Core.Providers;

namespace MT.Web.Example
{
    public class TenantObjectContext : TenantContext<TenantObject, int>
    {
        public TenantObjectContext(ITenantProvider<TenantObject, int> provider,
            DbContextOptions<TenantObjectContext> options) : base(provider, options)
        {
        }

        public DbSet<TenantObject> Ob { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TenantObject>();
            base.OnModelCreating(modelBuilder);
        }
    }
}