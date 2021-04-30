using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MT.Core.Extensions;

namespace MT.Web.Example
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDataProtection();
            services.AddDbContext<TenantObjectContext>(builder =>
                builder.UseSqlServer(
                    "Server=JMB-DK-07\\PUKACZ,6024;Database=MT;User Id=tenant;Password=tenant1234567;"));
            services.AddMultiTenancy<TenantCatalog, TenantObject, Guid>()
                .AddTenantCatalogContext<TenantCatalogContext>(builder => builder.UseInMemoryDatabase("dsa"))
                .AddTenantContext<TenantObjectContext>(builder => builder.UseInMemoryDatabase("dsa"));
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
}