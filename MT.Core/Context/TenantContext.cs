using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MT.Core.Model;
using MT.Core.Providers;
using SqlConnectionStringBuilder = System.Data.SqlClient.SqlConnectionStringBuilder;

namespace MT.Core.Context
{
    public class TenantContext : TenantContext<Tenant<string>, string>
    {
        public TenantContext(
            ITenantProvider<Tenant<string>, string> provider,
            DbContextOptions options)
            : base(provider, options)
        {
        }

        public TenantContext(
            DbContextOptions options)
            : base(options)
        {
        }
    }

    public class TenantContext<TTenant, TKey> : DbContext
        where TTenant : Tenant<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly SqlConnectionStringBuilder _connectionStringBuilder;
        private readonly ITenantProvider<TTenant, TKey> _provider;

        public TenantContext(ITenantProvider<TTenant, TKey> provider,
            DbContextOptions options)
            : base(options)
        {
            _provider = provider;
        }

        public TenantContext(SqlConnectionStringBuilder connectionStringBuilder, ITenantProvider<TTenant, TKey> provider)
        {
            _connectionStringBuilder = connectionStringBuilder;
            _provider = provider;
        }

        protected TenantContext(DbContextOptions connectionStringBuilder)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionStringBuilder != null)
            {
                var sqlConnection = new SqlConnection(_connectionStringBuilder.ConnectionString.Replace(@"""", ""));
                optionsBuilder.UseSqlServer(sqlConnection);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entityTypes = modelBuilder.Model.GetEntityTypes().Select(t => t.ClrType).ToList();
            foreach (var entityType in entityTypes)
            {
                ConfigureTenantEntity<ITenancy<TKey>>(modelBuilder, entityType);
            }
            base.OnModelCreating(modelBuilder);
        }

        private void ConfigureTenantEntity<TEntity>(ModelBuilder modelBuilder, Type entityType)
            where TEntity : ITenancy<TKey>
        {
            modelBuilder.Entity<TEntity>(builder =>
            {
                builder.ToTable(entityType.Name);
                builder.HasKey(entity => entity.Id);
                builder.Property(entity => entity.TenantId).IsRequired();
                builder.HasQueryFilter(filter => filter.TenantId.Equals(_provider.Get()));
            });
        }
    }
}