using Microsoft.EntityFrameworkCore;
using MT.Core.Context;

namespace MT.Web.Example
{
    public class OBCon : TenantContext<Ob, int, float>
    {
        public OBCon(DbContextOptions<OBCon> options) : base(options)
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