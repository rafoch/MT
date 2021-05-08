using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MT.Core.Context;
using MT.Core.Providers;
using MT.Core.Services;

namespace MT.Web.Example
{
    [Route("api/[controller]")]
    [ApiController]
    public class LolzController : ControllerBase
    {
        private readonly ExampleTenantCatalogDbContext _catalogDbContext;
        private readonly TenantManager<TenantCatalog, Guid> _manager;
        private readonly ITenantProvider<TenantCatalog, Guid> _provider;
        private readonly ITenantDbContextFactory<TenantDbObjectContext> _dbContextFactory;
        private readonly ITenantDbContextFactory<TenantDbObjectTwoContext> _dbContextFactory2;

        public LolzController(
            ExampleTenantCatalogDbContext catalogDbContext,
            TenantManager<TenantCatalog, Guid> manager,
            ITenantProvider<TenantCatalog, Guid> provider,
            ITenantDbContextFactory<TenantDbObjectContext> dbContextFactory,
            ITenantDbContextFactory<TenantDbObjectTwoContext> dbContextFactory2)
        {
            _catalogDbContext = catalogDbContext;
            _manager = manager;
            _provider = provider;
            _dbContextFactory = dbContextFactory;
            _dbContextFactory2 = dbContextFactory2;
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] TenantCatalog catalog)
        {
            await _manager.AddTenantAsync(catalog);
            return Ok(catalog);
        }

        [HttpGet]
        [Route("T")]
        public IActionResult T()
        {
            var list = _catalogDbContext.Tenants.ToList();
            return Ok(list);
        }

        [HttpGet]
        [Route("A")]
        public IActionResult Context([FromQuery] Guid tenantId)
        {
            _provider.Set(tenantId);
            var tenantObject = _dbContextFactory.Create();
            var first = tenantObject.TenantObject.ToList();
            return Ok(first);
        }

        [HttpGet]
        [Route("B")]
        public IActionResult ContextAdd([FromQuery] Guid tenantId)
        {
            _provider.Set(tenantId);
            var tenantContext = _dbContextFactory.Create();
            var entity = new TenantObject
            {
                TenantId = tenantId
            };
            tenantContext.TenantObject.Add(entity);
            tenantContext.SaveChanges();
            return Ok(entity);
        }

        [HttpGet]
        [Route("A2")]
        public IActionResult Context2([FromQuery] Guid tenantId)
        {
            _provider.Set(tenantId);
            var tenantObject = _dbContextFactory2.Create();
            var first = tenantObject.TenantObjectTwos.ToList();
            return Ok(first);
        }

        [HttpGet]
        [Route("B2")]
        public IActionResult ContextAdd2([FromQuery] Guid tenantId)
        {
            _provider.Set(tenantId);
            var tenantContext = _dbContextFactory2.Create();
            var entity = new TenantObjectTwo()
            {
                TenantId = tenantId
            };
            tenantContext.TenantObjectTwos.Add(entity);
            tenantContext.SaveChanges();
            return Ok(entity);
        }
    }
}