using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistentLayer.Auditable.Entities
{
    public abstract class PersistentEntity
    {
        public int Id { get; set; }
    }
}
