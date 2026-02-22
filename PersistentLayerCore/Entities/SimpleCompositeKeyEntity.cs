using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersistentLayerCore.Entities
{
    public class SimpleCompositeKeyEntity
    {
        [Key]
        [Column(Order = 1)]
        public int Id { get; set; }
        [Key]
        [Column(Order = 2)]
        public string Name { get; set; } = string.Empty;
    }
}
