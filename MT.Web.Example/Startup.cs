using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MT.Core.Extensions;
using MT.Core.InMemory.Extensions;

namespace MT.Web.Example
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<TenantDbObjectContext>(builder =>
                builder.UseSqlServer(
                    "Server=JMB-DK-07\\PUKACZ,6024;Database=MT;User Id=tenant;Password=tenant1234567;"));
        services.AddMultiTenancy<TenantCatalog, Guid>()
            .AddTenantCatalogContext<ExampleTenantCatalogDbContext>(builder => builder.UseInMemoryDatabase("test"))
            .AddTenantContext<TenantDbObjectContext>(builder => builder.UseInMemoryDatabase("test"))
            .AddTenantContext<TenantDbObjectTwoContext>(builder => builder.UseInMemoryDatabase("test"))
            .UseInMemory();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // app.UseMultiTenancy<TenantCatalog, Guid>();
            // app.MigrateTenantDatabases<TenantCatalog, Guid>();
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