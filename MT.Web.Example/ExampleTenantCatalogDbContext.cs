using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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