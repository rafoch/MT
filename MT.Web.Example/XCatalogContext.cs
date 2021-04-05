using Microsoft.EntityFrameworkCore;
using MT.Core.Context;

namespace MT.Web.Example
{
    public class XCatalogContext : TenantCatalogContext<X,int>
    {
        public XCatalogContext(DbContextOptions<XCatalogContext> options) : base(options)
        {
        }
    }
}