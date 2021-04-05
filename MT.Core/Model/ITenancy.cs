using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MT.Core.Model
{
    public abstract class ITenancy : ITenancy<string>
    {

    }

    public abstract class ITenancy<TKey>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id { get; set; }
        public TKey TenantId { get; set; }
    }
}