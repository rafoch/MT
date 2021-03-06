![example workflow](https://github.com/rafoch/MT/actions/workflows/dotnet.yml/badge.svg)
[![Nuget](https://img.shields.io/nuget/v/Multitenancy.Core?color=green&label=Multitenancy.Core)](https://www.nuget.org/packages/MultiTenancy.Core/)
[![Nuget](https://img.shields.io/nuget/dt/MultiTenancy.Core)](https://www.nuget.org/packages/MultiTenancy.Core/)
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

## Usage specific tenant context
```csharp
    public class TestUsageOftenantContextService
    {
        private readonly ITenantDbContextFactory<TenantDbObjectContext> _factory;

        public TestUsageOftenantContextService(ITenantDbContextFactory<TenantDbObjectContext> factory)
        {
            _factory = factory;
        }

        public void DoSomething()
        {
            // Be aware that before this u need to set TenantId in ITenantProvider
            using (var context = _factory.Create())
            {
                //do some stuff
            }
        }
    }
```

# Database Connection Support
| Name | Supported |
| :--: | :--: |
| SqlServer | ??? |
| PostgreSQL | ??? |
| MySQL | ??? |
