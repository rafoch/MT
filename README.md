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
            services.AddMultiTenancy<TenantCatalog, TenantObject, Guid>()
                .AddTenantCatalogContext<TenantCatalogContext>(builder => builder.UseSqlServer("dsa"))
                .AddTenantContext<TenantObjectContext>()
                .MigrateTenantContexts();
        }
```

## Create Tenant Catalog object context

```csharp
    public class TenantCatalogContext : TenantCatalogContext<TenantCatalog, Guid>
    {
        public TenantCatalogContext(DbContextOptions<TenantCatalogContext> options) : base(options)
        {
        }
    }
```

## Create tenant Object Context
We need to create our context that will be used across the application for retrivng and inserting data for various clients.
> :warning: **It's important to implement DbContext with two constructors** - One is for migrations and second is for using the application
```csharp
    public class TenantObjectContext : TenantContext<TenantObject, Guid>
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
    public class TenantObject : ITenancy<Guid>
    {
    }
```

# Database Connection Support
| Name | Supported |
| :--: | :--: |
| SqlServer | ✔ |
| PostgreSQL | ❌ |
| MySQL | ❌ |
