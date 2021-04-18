using System;

namespace MT.Core.Exceptions
{
    public class TenantNotProvidedException : Exception
    {
    }

    public class TenantNotFoundException : Exception
    {

        public TenantNotFoundException(string tenantId)
            :base($"Tenant with id: {tenantId} does not exist")
        {
            
        }
    }
}