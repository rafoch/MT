using System;
using FluentAssertions;
using MT.Core.Model;
using Xunit;

namespace MT.Core.UnitTests
{
    public class ObjectShouldImplementAbstractClass
    {
        [Fact]
        public void ObjectShouldImplementTenantClassWithTypedId()
        {
            var testTenantCatalogClass = new TestTenantCatalogClass();
            var typeAttributes = testTenantCatalogClass.GetType().GetProperties();
            var idAttribute = typeAttributes[0].PropertyType;
            idAttribute.Should().BeAssignableTo<Guid>();
            typeAttributes.Length.Should().Be(9);
        }

        [Fact]
        public void ObjectShouldImplementTenancyClassWithTypedId()
        {
            var testTenantCatalogClass = new TestTenancyClass();
            var typeAttributes = testTenantCatalogClass.GetType().GetProperties();
            var idAttribute = typeAttributes[0].PropertyType;
            idAttribute.Should().BeAssignableTo<Guid>();
            typeAttributes.Length.Should().Be(2);
        }

        private class TestTenantCatalogClass : Tenant<Guid>
        {
        }

        private class TestTenancyClass : ITenancy<Guid>
        {
        }
    }
}
