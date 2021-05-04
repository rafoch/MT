using System;
using Microsoft.AspNetCore.Builder;
using MT.Core.Middlewares;
using MT.Core.Model;
using MT.Core.Providers;

namespace MT.Core.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void UseMultiTenancy<TTenant, TKey>(this IApplicationBuilder builder)
        where TTenant : Tenant<TKey> 
        where TKey : IEquatable<TKey>
        {
            // var tenantProvider = new TenantProvider<TTenant, TKey>();
            builder.UseMiddleware<TenantHttpMiddleware<TTenant,TKey>>();
        }
    }
}