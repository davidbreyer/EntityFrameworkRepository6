using EntityFrameworkAuditableRepository6.Base;
using PersistentLayerAuditable.Contexts;
using PersistentLayerAuditable.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistentLayerAuditable.Repositories
{
    public interface ISimpleDataEntityRepository : IBaseRepository<YourCustomDataContext, SimpleDataEntity>
    {
    }

    public class SimpleDataEntityRepository : BaseRepository<YourCustomDataContext, SimpleDataEntity>, ISimpleDataEntityRepository
    {
        public SimpleDataEntityRepository(IDatabaseFactory<YourCustomDataContext> dbFactory)
        {
            Context = dbFactory.GetNewDbContext();
        }
    }
}
