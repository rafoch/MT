using System;
using MT.Core.Context;

namespace MT.Core.Interfaces
{
    /// <inheritdoc />
    public interface ITenantDbContextFactory<out TContext> : IDisposable
    {
        /// <summary>
        /// Creates delivered <see cref="TenantDbContext"/>
        /// </summary>
        /// <returns>Return dbContext that inherits from <see cref="TenantDbContext"/></returns>
        TContext Create();
    }
}