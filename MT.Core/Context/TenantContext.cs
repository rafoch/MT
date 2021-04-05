using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MT.Core.Model;

namespace MT.Core.Context
{
    public class TenantContext : DbContext
    {
        public TenantContext(DbContextOptions options)
            :base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List<Type> types = new List<Type>();
            types.AddRange(modelBuilder.Model.GetEntityTypes().Select(t => t.ClrType).ToList());
            foreach (var entityType in types)
            {
                ConfigureTenantEntity<ITenancy>(entityType, modelBuilder);
            }
            base.OnModelCreating(modelBuilder);
        }

        public static void ConfigureTenantEntity<TEntity>(Type entity, ModelBuilder modelBuilder)
            where TEntity : ITenancy
        {
            modelBuilder.Entity<TEntity>(builder =>
            {
                builder.HasQueryFilter(filter => filter.TenantId == "5801E77E-36F0-4F3C-9423-82890C0E3B9A".ToLower()); //todo Tenant ID provider
            });
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}