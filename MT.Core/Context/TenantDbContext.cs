using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using MT.Core.Interfaces;
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
            ITenantProvider<Tenant<string>, string> tenantProvider,
            DbContextOptions options)
            : base(tenantProvider, options)
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
    public abstract class TenantDbContext<TTenant, TKey> : DbContext
        where TTenant : Tenant<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly SqlConnectionStringBuilder _connectionStringBuilder;
        private readonly ITenantProvider<TTenant, TKey> _tenantProvider;
        private readonly IOnConfiguringDbContextOptionsBuilderProvider _onConfiguringDbContextOptionsBuilderProvider;

        /// <inheritdoc />
        protected TenantDbContext(
            ITenantProvider<TTenant, TKey> tenantProvider,
            DbContextOptions options)
            : base(options)
        {
            _tenantProvider = tenantProvider;
        }

        /// <inheritdoc />
        public TenantDbContext(
            [NotNull] SqlConnectionStringBuilder connectionStringBuilder,
            [NotNull] ITenantProvider<TTenant, TKey> tenantProvider,
            [NotNull]  IOnConfiguringDbContextOptionsBuilderProvider onConfiguringDbContextOptionsBuilderProvider)
        {
            _connectionStringBuilder = connectionStringBuilder;
            _tenantProvider = tenantProvider;
            _onConfiguringDbContextOptionsBuilderProvider = onConfiguringDbContextOptionsBuilderProvider;
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
                _onConfiguringDbContextOptionsBuilderProvider.Provide(optionsBuilder, sqlConnection);
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
                builder.HasQueryFilter(filter => filter.TenantId.Equals(_tenantProvider.Get()));
            });
        }
    }
}