using System;
using FluentAssertions;
using Xunit;

namespace MT.Core.UnitTests
{
    public class ObjectShouldImplementAbstractClass : BaseTestClass
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

    }
}
