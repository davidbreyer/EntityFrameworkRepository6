using EntityFramework.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersistentLayerAuditable.Entities
{
    public class SimpleDataEntityAudit : AuditEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public override int AuditSourceId { get; set; }

        public string Name { get; set; }
    }
}
