using System;
using Microsoft.EntityFrameworkCore;
using MT.Core.Context;
using MT.Core.Model;

namespace MT.Core.UnitTests
{
    public class BaseTestClass
    {
        protected class TestTenantCatalogClass : Tenant<Guid>
        {
        }

        protected class TestTenancyClass : Tenancy<Guid>
        {
        }

        protected class TestTenantCatalogDbDbContextClass : TenantCatalogDbContext<TestTenantCatalogClass, Guid>
        {
            public TestTenantCatalogDbDbContextClass(DbContextOptions options) : base(options)
            {
            }


            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("test");
                base.OnConfiguring(optionsBuilder);
            }
        }
    }
}