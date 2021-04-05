using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MT.Core.Model;

namespace MT.Core.Context
{
    public class TenantCatalogContext : TenantCatalogContext<Tenant, string>
    {
        public TenantCatalogContext(DbContextOptions options) : base(options)
        {
        }
    }

    public class TenantCatalogContext<TUser, TKey> : DbContext
        where TUser : Tenant<TKey>
        where TKey : IEquatable<TKey>
    {
        public TenantCatalogContext(DbContextOptions options)
            :base(options)
        {

        }

        public DbSet<TUser> Tenants { get; set; }
    }

    public class TenantContext : DbContext
    {
        public TenantContext(DbContextOptions options)
            :base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var type = Assembly.GetEntryAssembly().GetType();
            var typeInfo = type.GetTypeInfo();
            var methodInfo = typeInfo.GetMethod("ConfigureTenantEntity", BindingFlags.Public | BindingFlags.Static);
            var declaredMethods = typeInfo.GetMethods(BindingFlags.Public | BindingFlags.Static);
            var configureMethod = declaredMethods.Single(m => m.Name == nameof(ConfigureTenantEntity));
            var args = new object[] { modelBuilder };
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                configureMethod.MakeGenericMethod(entityType.ClrType).Invoke(null, args);
            }
            base.OnModelCreating(modelBuilder);
        }

        public static void ConfigureTenantEntity<TEntity>(TEntity entity, ModelBuilder modelBuilder)
            where TEntity : ITenancy
        {
//            modelBuilder.Entity(typeof(TEntity), builder => builder.HasQueryFilter(filter => filter.))
            modelBuilder.Entity<TEntity>(builder =>
                builder.HasQueryFilter(filter => filter.TenantId == Guid.Empty.ToString()));
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
//            var executingAssembly = Assembly.GetCallingAssembly();
//            var propertyInfoses = executingAssembly.GetTypes()
//                .Where(t => t.IsClass &&
//                            !t.IsAbstract && 
//                            !t.IsInterface &&
//                            t.IsAssignableFrom(typeof(TenantContext))).SelectMany(t => t.Get());
//            var propertyInfos = propertyInfoses.Where(p => p.GetType().IsAssignableFrom(typeof(DbSet<>)));
            base.OnConfiguring(optionsBuilder);
        }
    }
}