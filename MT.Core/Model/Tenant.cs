using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MT.Core.Model
{
    public abstract class Tenant<TKey>
        where TKey : IEquatable<TKey>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual TKey Id { get; set; }
        public virtual string Server { get; set; }
        public virtual string Name { get; set; }
        public virtual string Password { get; set; }
        public virtual string NormalizedName { get; set; }
        public virtual string Port { get; set; }
        public virtual string Database { get; set; }
    }

    public abstract class Tenant : Tenant<string>
    {

    }
}