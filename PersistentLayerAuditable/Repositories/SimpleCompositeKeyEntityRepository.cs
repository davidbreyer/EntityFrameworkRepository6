using EntityFramework.Auditable.Repository6;
using EntityFramework.Repository6;
using EntityFramework.Repository6.Interfaces;
using PersistentLayer.Auditable.Contexts;
using PersistentLayer.Auditable.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistentLayer.Auditable.Repositories
{
    public interface ISimpleCompositeKeyEntityRepository : IBaseRepository<YourCustomDataContext, SimpleCompositeKeyEntity>
    {

    }

    public class SimpleCompositeKeyEntityRepository : AuditableBaseRepository<YourCustomDataContext, SimpleCompositeKeyEntity>, ISimpleCompositeKeyEntityRepository
    {
        public SimpleCompositeKeyEntityRepository(IDatabaseFactory<YourCustomDataContext> dbFactory) : base(dbFactory.GetNewDbContext())
        {
            
        }
    }
}
