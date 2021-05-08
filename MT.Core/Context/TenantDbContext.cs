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
    /// <inheritdoc />
    public class TenantDbContext : TenantDbContext<Tenant<string>, string>
    {
        /// <inheritdoc />
        public TenantDbContext(
            ITenantProvider<Tenant<string>, string> provider,
            DbContextOptions options)
            : base(provider, options)
        {
        }

        /// <inheritdoc />
        public TenantDbContext(
            DbContextOptions options)
            : base(options)
        {
        }
    }

    /// <inheritdoc />
    public class TenantDbContext<TTenant, TKey> : DbContext
        where TTenant : Tenant<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly SqlConnectionStringBuilder _connectionStringBuilder;
        private readonly ITenantProvider<TTenant, TKey> _provider;

        /// <inheritdoc />
        public TenantDbContext(ITenantProvider<TTenant, TKey> provider,
            DbContextOptions options)
            : base(options)
        {
            _provider = provider;
        }

        /// <inheritdoc />
        protected TenantDbContext(SqlConnectionStringBuilder connectionStringBuilder, ITenantProvider<TTenant, TKey> provider)
        {
            _connectionStringBuilder = connectionStringBuilder;
            _provider = provider;
        }

        /// <inheritdoc />
        protected TenantDbContext(DbContextOptions connectionStringBuilder)
        {
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionStringBuilder != null)
            {
                var sqlConnection = new SqlConnection(_connectionStringBuilder.ConnectionString.Replace(@"""", ""));
                optionsBuilder.UseSqlServer(sqlConnection.ConnectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entityTypes = modelBuilder.Model.GetEntityTypes().Select(t => t.ClrType).ToList();
            foreach (var entityType in entityTypes)
            {
                ConfigureTenantEntity<Tenancy<TKey>>(modelBuilder, entityType);
            }
            base.OnModelCreating(modelBuilder);
        }

        private void ConfigureTenantEntity<TEntity>(ModelBuilder modelBuilder, Type entityType)
            where TEntity : Tenancy<TKey>
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