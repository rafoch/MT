using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MT.Core.Context;
using MT.Core.Extensions;
using MT.Core.Model;

namespace MT.Web.Example
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMultiTenancyCatalog<XCatalogContext>(builder => builder.UseInMemoryDatabase("dsa"));
            services.AddMultiTenancyContext<OBCon>(builder => builder.UseInMemoryDatabase("dsa"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class X : Tenant<int>
    {

    }

    public class XCatalogContext : TenantCatalogContext<X,int>
    {
        public XCatalogContext(DbContextOptions<XCatalogContext> options) : base(options)
        {
        }
    }

    public class Ob : ITenancy
    {
        [Key]
        public Guid Id { get; set; }
    }

    public class OBCon : TenantContext
    {
        public OBCon(DbContextOptions<OBCon> options) : base(options)
        {
        }

        public DbSet<Ob> Ob { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class LolzController : ControllerBase
    {
        private readonly XCatalogContext _catalogContext;
        private readonly OBCon _obCon;

        public LolzController(
            XCatalogContext catalogContext,
            OBCon obCon)
        {
            _catalogContext = catalogContext;
            _obCon = obCon;
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
                TenantId = Guid.NewGuid().ToString()
            });
            _obCon.SaveChanges();
            var list = _obCon.Ob.ToList();
            return Ok(list);
        }
    }
}
