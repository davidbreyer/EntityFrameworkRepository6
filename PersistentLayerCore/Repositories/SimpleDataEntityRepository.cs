using EntityFramework.Repository.Core;
using EntityFramework.Repository.Core.Interfaces;
using PersistentLayerCore.Contexts;
using PersistentLayerCore.Entities;

namespace PersistentLayerCore.Repositories
{
    public interface ISimpleDataEntityRepository : IBaseRepository<YourCustomDataContext, SimpleDataEntity>
    {
    }

    public class SimpleDataEntityRepository : BaseRepository<YourCustomDataContext, SimpleDataEntity>, ISimpleDataEntityRepository
    {
        public SimpleDataEntityRepository(IDatabaseFactory<YourCustomDataContext> dbFactory) : base(dbFactory.GetNewDbContext())
        {
        }
    }
}
