using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MT.Core.Providers;
using MT.Core.Services;

namespace MT.Web.Example
{
    [Route("api/[controller]")]
    [ApiController]
    public class LolzController : ControllerBase
    {
        private readonly TenantCatalogContext _catalogContext;
        private readonly TenantObjectContext _tenantObjectContext;
        private readonly TenantManager<TenantCatalog, int> _manager;
        private readonly ITenantProvider<TenantObject, int> _provider;

        private readonly Guid _testGuid;
        // UserManager<>

        public LolzController(
            TenantCatalogContext catalogContext,
            TenantObjectContext tenantObjectContext,
            TenantManager<TenantCatalog, int> manager,
            ITenantProvider<TenantObject, int> provider)
        {
            _catalogContext = catalogContext;
            _tenantObjectContext = tenantObjectContext;
            _manager = manager;
            _provider = provider;
            _testGuid = new Guid("5801E77E-36F0-4F3C-9423-82890C0E3B9A");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tenant = new TenantCatalog()
            {
                Password = "super password"
            };
            await _manager.AddTenantAsync(tenant);
            return Ok(tenant);
        }

        [HttpGet]
        [Route("T")]
        public IActionResult T()
        {
            var list = _catalogContext.Tenants.ToList();
            return Ok(list);
        }

        [HttpGet]
        [Route("TenantCatalog")]
        public IActionResult X([FromQuery] int tenantId)
        {
            _tenantObjectContext.Ob.Add(new TenantObject
            {
                TenantId = tenantId
            });
            _tenantObjectContext.SaveChanges();
            _provider.Set(tenantId);
            var obs = _tenantObjectContext.Ob.IgnoreQueryFilters().ToList();
            var list = _tenantObjectContext.Ob.ToList();
            return Ok(list);
        }
    }
}