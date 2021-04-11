using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MT.Core.Model;

namespace MT.Core.Context
{
    public class TenantContext : TenantContext<ITenancy, string>
    {
        public TenantContext(DbContextOptions options)
            :base(options)
        {
        }
        }

    public class TenantContext<TUser, TKey> : TenantContext<TUser, TKey, string>
        where TUser : ITenancy<TKey>
        where TKey : IEquatable<TKey>
    {
        public TenantContext(DbContextOptions options)
            :base(options)
        {
            
        }
    }

    public class TenantContext<TUser, TKey, TTenantKey> : DbContext
        where TUser : ITenancy<TKey, TTenantKey>
        where TKey : IEquatable<TKey>
    {
        public TenantContext(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach(var entityType in modelBuilder.Model.GetEntityTypes().Select(t => t.ClrType))
            {
                ConfigureTenantEntity<ITenancy<TKey, TTenantKey>>(entityType, modelBuilder);
            }
            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigureTenantEntity<TEntity>(Type entity, ModelBuilder modelBuilder)
            where TEntity : ITenancy<TKey, TTenantKey>
        {
            modelBuilder.Entity<TEntity>(builder =>
            {
                builder.HasQueryFilter(filter => EqualityComparer<TKey>.Default.Equals(filter.Id, filter.Id)); //todo Tenant ID provider
            });
        }
    }
}