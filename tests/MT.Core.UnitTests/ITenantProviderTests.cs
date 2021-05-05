using System;
using FluentAssertions;
using MT.Core.Providers;
using Xunit;

namespace MT.Core.UnitTests
{
    public class ITenantProviderTests : BaseTestClass
    {
        private readonly TenantProvider<TestTenantCatalogClass, Guid> _tenantProvider;

        public ITenantProviderTests()
        {
            _tenantProvider = new TenantProvider<TestTenantCatalogClass, Guid>();
        }

        [Fact]
        public void ShouldSetTenantIdInProvider()
        {
            _tenantProvider.Set(Guid.NewGuid());
        }

        [Fact]
        public void ShouldGetTenantIdInProvider()
        {
            var newGuid = Guid.NewGuid();
            _tenantProvider.Set(newGuid);
            _tenantProvider.Get().Should().Be(newGuid);
        }
    }
}