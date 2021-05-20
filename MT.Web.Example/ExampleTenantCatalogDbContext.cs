using System;
using Microsoft.EntityFrameworkCore;
using MT.Core.Context;

namespace MT.Web.Example
{
    public class ExampleTenantCatalogDbContext : TenantCatalogDbContext<TenantCatalog, Guid>
    {
        public ExampleTenantCatalogDbContext(DbContextOptions<ExampleTenantCatalogDbContext> options) : base(options)
        {
        }
    }
}