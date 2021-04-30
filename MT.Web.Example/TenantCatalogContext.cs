using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MT.Core.Context;

namespace MT.Web.Example
{
    public class TenantCatalogContext : TenantCatalogContext<TenantCatalog, Guid>
    {
        public TenantCatalogContext(DbContextOptions<TenantCatalogContext> options) : base(options)
        {
        }
    }
}