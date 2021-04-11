using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MT.Core.Model
{
    public abstract class ITenancy : ITenancy<string>
    {

    }

    public abstract class ITenancy<TKey> : ITenancy<TKey, string>
    {
        
    }

    public abstract class ITenancy<TKey, ITenantKey>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id { get; set; }
        public ITenantKey TenantId { get; set; }
    }
}