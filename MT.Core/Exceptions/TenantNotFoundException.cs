using System;

namespace MT.Core.Exceptions
{
    /// <inheritdoc />
    public class TenantNotFoundException : Exception
    {
        /// <inheritdoc />
        public TenantNotFoundException(string tenantId)
            :base($"Tenant with id: {tenantId} does not exist")
        {
        }
    }
}