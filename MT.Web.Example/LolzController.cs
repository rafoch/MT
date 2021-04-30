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
        private readonly ITenantProvider<TenantObject, Guid> _provider;
        private readonly ITenantContextFactory<TenantObjectContext> _contextFactory;

        public LolzController(
            TenantCatalogContext catalogContext,
            TenantManager<TenantCatalog, Guid> manager,
            ITenantProvider<TenantObject, Guid> provider,
            ITenantContextFactory<TenantObjectContext> contextFactory)
        {
            _catalogContext = catalogContext;
            _manager = manager;
            _provider = provider;
            _contextFactory = contextFactory;
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
    }
}