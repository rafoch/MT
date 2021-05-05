using System;
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
    }
}