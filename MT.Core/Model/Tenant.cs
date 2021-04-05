using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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

    public abstract class Tenant : Tenant<string>
    {

    }
}