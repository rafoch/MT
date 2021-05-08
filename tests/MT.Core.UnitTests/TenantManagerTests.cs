using System;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MT.Core.Context;
using MT.Core.Exceptions;
using MT.Core.Services;
using MT.Core.Validators;
using Xunit;

namespace MT.Core.UnitTests
{
    public class TenantManagerTests : BaseTestClass
    {
        private readonly TenantManager<TestTenantCatalogClass, Guid> _manager;

        public TenantManagerTests()
        {
            _manager = new TenantManager<TestTenantCatalogClass, Guid>(
                new TestTenantCatalogDbDbContextClass(new DbContextOptions<TestTenantCatalogDbDbContextClass>()),
                new TenantValidator<TestTenantCatalogClass, Guid>());
        }
        [Fact]
        public void ShouldAddSynchronouslyTenantToDatabase()
        {
            var tenantCatalogClass = new TestTenantCatalogClass()
            {
                Password = "test",
                Server = "test",
                Database = "test"
            };

            var testTenantCatalogClass = _manager.AddTenant(tenantCatalogClass);
            testTenantCatalogClass.Should().NotBeNull();
        }

        [Fact]
        public async void ShouldAddAsynchronouslyTenantToDatabase()
        {
            var testTenantCatalogClass = new TestTenantCatalogClass()
            {
                Password = "test",
                Server = "test",
                Database = "test"
            };
            var addTenantAsync = await _manager.AddTenantAsync(testTenantCatalogClass);
            addTenantAsync.Should().NotBeNull();
        }

        [Fact]
        public void ShouldThrowPasswordIsMissingExceptionOnSyncAddTenantMethod()
        {
            Assert.Throws<TenantDatabasePasswordIsMissingException>(() =>
                _manager.AddTenant(new TestTenantCatalogClass()));
        }

        [Fact]
        public async void ShouldThrowPasswordIsMissingExceptionOnAsyncAddTenantMethod()
        {
            await Assert.ThrowsAsync<TenantDatabasePasswordIsMissingException>(async () =>
                await _manager.AddTenantAsync(new TestTenantCatalogClass()));
        }

        [Fact]
        public void ShouldThrowTenantObjectIsMissingExceptionOnAddTenantSyncMethodWhenNullIsProvided()
        {
            Assert.Throws<TenantObjectIsMissingException>(() => _manager.AddTenant(null));
        }
    }
}