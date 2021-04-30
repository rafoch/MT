using System;
using System.ComponentModel.DataAnnotations;
using MT.Core.Model;

namespace MT.Web.Example
{
    public class TenantObject : ITenancy<Guid>
    {
        public string Name { get; set; }
    }
}