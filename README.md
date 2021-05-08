![example workflow](https://github.com/rafoch/MT/actions/workflows/dotnet.yml/badge.svg)
![Shields.io](https://img.shields.io/nuget/v/MultiTenancy.Core?color=dd&label=MultiTenancy.Core)
![Shields.io](https://img.shields.io/nuget/dt/MultiTenancy.Core?label=downloads)
# MultiTenancy
a lightway package to manage and implement multi tenant architecture to your .NET Core Applications

# Usage

## Usage in Startup

```csharp
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMultiTenancy<TenantCatalog, Guid>()
                .AddTenantCatalogContext<TenantCatalogContext>(builder => builder.UseSqlServer("<Your connection string>"))
                .AddTenantContext<TenantObjectContext>();
        }
```

```csharp
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Creates middleware that search for tenant-id in Header for each request and set it's to TenantProvider
            app.UseMultiTenancy<TenantCatalog, Guid>();
            // Use this if you want to migrate all tenants databases specified in Catalog Database
            app.MigrateTenantDatabases<TenantCatalog, Guid>(); 
        }
```

## Create Tenant Catalog object context

```csharp
    public class ExampleTenantCatalogDbContext : TenantCatalogDbContext<TenantCatalog, Guid>
    {
        public ExampleTenantCatalogDbContext(DbContextOptions<ExampleTenantCatalogDbContext> options) : base(options)
        {
        }
    }
```

## Create tenant Object Context
We need to create our context that will be used across the application for retrivng and inserting data for various clients.
> :warning: **It's important to implement DbContext with two constructors** - One is for migrations and second is for using the application
```csharp
    public class TenantObjectContext : TenantDbContext<TenantCatalog, Guid>
    {
        public TenantObjectContext(ITenantProvider<TenantObject, Guid> provider,
            DbContextOptions<TenantObjectContext> options) : base(provider, options)
        {
        }

        public TenantObjectContext(
            SqlConnectionStringBuilder connectionStringBuilder, 
            ITenantProvider<TenantObject, Guid> provider) 
            : base(connectionStringBuilder, provider)
        {
        }
    }
```

## Create tenant catalog object
```csharp
    public class TenantCatalog : Tenant<Guid>
    {
    }
```
## Create tenant object 
```csharp
    public class TenantObject : Tenancy<Guid>
    {
    }
```

# Database Connection Support
| Name | Supported |
| :--: | :--: |
| SqlServer | ✔ |
| PostgreSQL | ❌ |
| MySQL | ❌ |
