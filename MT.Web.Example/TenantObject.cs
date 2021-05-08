using System;
using System.ComponentModel.DataAnnotations;
using MT.Core.Model;

namespace MT.Web.Example
{
    public class TenantObject : Tenancy<Guid>
    {
        public string Name { get; set; }
    }

    public class TenantObjectTwo : Tenancy<Guid>
    {
        public string Name { get; set; }
    }
}