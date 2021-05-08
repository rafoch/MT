using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MT.Core.Context;

namespace MT.Core.Model
{
    /// <inheritdoc />
    public abstract class Tenancy : Tenancy<string>
    {

    }

    /// <summary>
    /// Class that contains <see cref="Id"/> and <see cref="TenantId"/> variables.
    /// It is used to store and retrieve data from <see cref="TenantDbContext{TTenant,TKey}"/> 
    /// </summary>
    /// <typeparam name="TKey"><see cref="Id"/></typeparam>
    public abstract class Tenancy<TKey>
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id { get; set; }
        /// <summary>
        /// Tenant Id
        /// </summary>
        public TKey TenantId { get; set; }
    }
}