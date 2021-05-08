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

        protected class TestTenancyClass : ITenancy<Guid>
        {
        }

        protected class TestTenantCatalogDbContextClass : TenantCatalogContext<TestTenantCatalogClass, Guid>
        {
            public TestTenantCatalogDbContextClass(DbContextOptions options) : base(options)
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