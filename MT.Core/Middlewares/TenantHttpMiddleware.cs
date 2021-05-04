using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MT.Core.Model;
using MT.Core.Providers;

namespace MT.Core.Middlewares
{
    /// <summary>
    /// Adds typical middleware that search in request Headers section for key <see cref="Constans.Constans.TenantIdHeaderKey"/>
    /// and setting it into <see cref="ITenantProvider{TTenant,TKey}"/>
    /// </summary>
    /// <typeparam name="TTenant">An object that implements <see cref="Tenant{TKey}"/> type</typeparam>
    /// <typeparam name="TKey">An <see cref="Tenant{TKey}"/> identifier type</typeparam>
    public class TenantHttpMiddleware<TTenant, TKey> 
        where TTenant : Tenant<TKey> 
        where TKey : IEquatable<TKey>
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantHttpMiddleware<TTenant, TKey>> _logger;

        public TenantHttpMiddleware(
            RequestDelegate next,
            ILogger<TenantHttpMiddleware<TTenant, TKey>> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext, ITenantProvider<TTenant, TKey> provider)
        {
            var requestHeader = httpContext.Request.Headers[Constans.Constans.TenantIdHeaderKey];
            if (requestHeader.Count != 0)
            {
                var key = (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(requestHeader[0]);
                provider.Set(key);
            }

            await _next(httpContext);
        }
    }
}