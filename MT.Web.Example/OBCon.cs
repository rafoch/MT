using Microsoft.EntityFrameworkCore;
using MT.Core.Context;
using MT.Core.Providers;

namespace MT.Web.Example
{
    public class OBCon : TenantContext<Ob, int>
    {
        public OBCon(ITenantProvider<Ob, int> provider,
            DbContextOptions<OBCon> options) : base(provider, options)
        {
        }

        public DbSet<Ob> Ob { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ob>();
            base.OnModelCreating(modelBuilder);
        }
    }
}