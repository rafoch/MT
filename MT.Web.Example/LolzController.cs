using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MT.Web.Example
{
    [Route("api/[controller]")]
    [ApiController]
    public class LolzController : ControllerBase
    {
        private readonly XCatalogContext _catalogContext;
        private readonly OBCon _obCon;
        private readonly Guid _testGuid;

        public LolzController(
            XCatalogContext catalogContext,
            OBCon obCon)
        {
            _catalogContext = catalogContext;
            _obCon = obCon;
            _testGuid = new Guid("5801E77E-36F0-4F3C-9423-82890C0E3B9A");
        }

        [HttpGet]
        public IActionResult Index()
        {
            var list = _catalogContext.Tenants.ToList();
            return Ok(list);
        }

        [HttpGet]
        [Route("T")]
        public IActionResult T()
        {
            _catalogContext.Tenants.Add(new X());
            _catalogContext.SaveChanges();
            var list = _catalogContext.Tenants.ToList();
            return Ok(list);
        }

        [HttpGet]
        [Route("X")]
        public IActionResult X()
        {
            _obCon.Ob.Add(new Ob
            {
                TenantId = _testGuid.ToString()
            });
            _obCon.SaveChanges();
            var obs = _obCon.Ob.IgnoreQueryFilters().ToList();
            var list = _obCon.Ob.ToList();
            return Ok(list);
        }
    }
}