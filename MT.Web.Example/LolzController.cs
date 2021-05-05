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
        private readonly TenantCatalogContext _catalogContext;
        private readonly TenantManager<TenantCatalog, Guid> _manager;
        private readonly ITenantProvider<TenantCatalog, Guid> _provider;
        private readonly ITenantContextFactory<TenantObjectContext> _contextFactory;
        private readonly ITenantContextFactory<TenantObjectTwoContext> _contextFactory2;

        public LolzController(
            TenantCatalogContext catalogContext,
            TenantManager<TenantCatalog, Guid> manager,
            ITenantProvider<TenantCatalog, Guid> provider,
            ITenantContextFactory<TenantObjectContext> contextFactory,
            ITenantContextFactory<TenantObjectTwoContext> contextFactory2)
        {
            _catalogContext = catalogContext;
            _manager = manager;
            _provider = provider;
            _contextFactory = contextFactory;
            _contextFactory2 = contextFactory2;
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
            var list = _catalogContext.Tenants.ToList();
            return Ok(list);
        }

        [HttpGet]
        [Route("A")]
        public IActionResult Context([FromQuery] Guid tenantId)
        {
            _provider.Set(tenantId);
            var tenantObject = _contextFactory.Create();
            var first = tenantObject.TenantObject.ToList();
            return Ok(first);
        }

        [HttpGet]
        [Route("B")]
        public IActionResult ContextAdd([FromQuery] Guid tenantId)
        {
            _provider.Set(tenantId);
            var tenantContext = _contextFactory.Create();
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
            var tenantObject = _contextFactory2.Create();
            var first = tenantObject.TenantObjectTwos.ToList();
            return Ok(first);
        }

        [HttpGet]
        [Route("B2")]
        public IActionResult ContextAdd2([FromQuery] Guid tenantId)
        {
            _provider.Set(tenantId);
            var tenantContext = _contextFactory2.Create();
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