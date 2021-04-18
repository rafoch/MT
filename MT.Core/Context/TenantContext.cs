using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MT.Core.Model;
using MT.Core.Providers;

namespace MT.Core.Context
{
    public class TenantContext : TenantContext<ITenancy<string>, string>
    {
        public TenantContext(ITenantProvider<ITenancy<string>, string> provider,
            DbContextOptions options)
            : base(provider, options)
        {
        }
    }

    public class TenantContext<TUser, TKey> : DbContext
        where TUser : ITenancy<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly ITenantProvider<TUser, TKey> _provider;

        public TenantContext(ITenantProvider<TUser, TKey> provider,
            DbContextOptions options)
            : base(options)
        {
            _provider = provider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entityTypes = modelBuilder.Model.GetEntityTypes().Select(t => t.ClrType).ToList();
            foreach (var entityType in entityTypes)
            {
                ConfigureTenantEntity<ITenancy<TKey>>(entityType, modelBuilder);
            }
            base.OnModelCreating(modelBuilder);
        }

        private void ConfigureTenantEntity<TEntity>(Type entity, ModelBuilder modelBuilder)
            where TEntity : ITenancy<TKey>
        {
            modelBuilder.Entity<TEntity>(builder =>
            {
                builder.HasQueryFilter(filter => EqualityComparer<TKey>.Default.Equals(filter.TenantId, _provider.Get()));
            });
        }
    }
}