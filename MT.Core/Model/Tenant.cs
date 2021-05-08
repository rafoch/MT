using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MT.Core.Services;

namespace MT.Core.Model
{
    /// <summary>
    /// Object that represent Tenant in database
    /// </summary>
    /// <typeparam name="TKey"><see cref="Id"/></typeparam>
    public abstract class Tenant<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Creates new instance of <see cref="Tenant{TKey}"/>
        /// </summary>
        protected Tenant()
        {
            ConcurrencyStamp = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual TKey Id { get; set; }
        
        /// <summary>
        /// Tenant database server name. It is used to create connection string to database
        /// </summary>
        public virtual string Server { get; set; }
        /// <summary>
        /// Tenant name in the database
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// UserName to <see cref="Server"/>
        /// </summary>
        public virtual string UserName { get; set; }
        /// <summary>
        /// Password to <see cref="Server"/>. Password is hashed via <see cref="TenantManager{TTenant,TKey}.EncryptionHelper.Encrypt"/>
        /// </summary>
        public virtual string Password { get; set; }
        /// <summary>
        /// Value that is used to encrypt and decrypt password
        /// </summary>
        public virtual string ConcurrencyStamp { get; set; }

        /// <summary>
        /// Represent <see cref="Name"/> in Capitalized value
        /// </summary>
        public virtual string NormalizedName => Name?.ToUpperInvariant();
        
        /// <summary>
        /// Port to <see cref="Server"/>
        /// </summary>
        public virtual string Port { get; set; }
        /// <summary>
        /// Database name in <see cref="Server"/>
        /// </summary>
        public virtual string Database { get; set; }
    }

    /// <inheritdoc />
    public abstract class Tenant : Tenant<string>
    {

    }
}