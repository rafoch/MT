using System;
using System.ComponentModel.DataAnnotations;

namespace MT.Core.Model
{
    public class Tenant<TKey> where TKey : IEquatable<TKey>
    {
        [Key]
        public TKey Id { get; set; }
        public string Server { get; set; }
        public string Port { get; set; }
        public string Database { get; set; }
    }

    public class Tenant : Tenant<string>
    {

    }

    public abstract class ITenancy : ITenancy<string>
    {

    }

    public abstract class ITenancy<TKey>
    {
        public TKey TenantId { get; set; }
    }
}