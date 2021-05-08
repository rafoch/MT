using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MT.Core.Model;

namespace MT.Core.Context
{
    /// <inheritdoc />
    public class TenantCatalogDbContext : TenantCatalogDbContext<Tenant, string>
    {
        /// <inheritdoc />
        public TenantCatalogDbContext(DbContextOptions options) : base(options)
        {
        }
    }

    /// <inheritdoc />
    public class TenantCatalogDbContext<TTenant, TKey> : DbContext
        where TTenant : Tenant<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <inheritdoc />
        public TenantCatalogDbContext(DbContextOptions options)
            : base(options)
        {

        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TTenant>(builder =>
            {
                builder.HasKey(user => user.Id);
            });
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Collection that represents all the tenants in your application
        /// </summary>
        public DbSet<TTenant> Tenants { get; set; }
    }

}