using System;

namespace MT.Core.Context
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