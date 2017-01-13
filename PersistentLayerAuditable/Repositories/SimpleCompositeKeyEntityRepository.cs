using EntityFrameworkAuditableRepository6.Base;
using PersistentLayerAuditable.Contexts;
using PersistentLayerAuditable.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistentLayerAuditable.Repositories
{
    public interface ISimpleCompositeKeyEntityRepository : IBaseRepository<YourCustomDataContext, SimpleCompositeKeyEntity>
    {

    }

    public class SimpleCompositeKeyEntityRepository : BaseRepository<YourCustomDataContext, SimpleCompositeKeyEntity>, ISimpleCompositeKeyEntityRepository
    {
        public SimpleCompositeKeyEntityRepository(IDatabaseFactory<YourCustomDataContext> dbFactory)
        {
            Context = dbFactory.GetNewDbContext();
        }
    }
}
