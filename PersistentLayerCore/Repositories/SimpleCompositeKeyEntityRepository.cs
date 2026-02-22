using EntityFramework.Repository.Core;
using EntityFramework.Repository.Core.Interfaces;
using PersistentLayerCore.Contexts;
using PersistentLayerCore.Entities;

namespace PersistentLayerCore.Repositories
{
    public interface ISimpleCompositeKeyEntityRepository : IBaseRepository<YourCustomDataContext, SimpleCompositeKeyEntity>
    {
    }

    public class SimpleCompositeKeyEntityRepository : BaseRepository<YourCustomDataContext, SimpleCompositeKeyEntity>, ISimpleCompositeKeyEntityRepository
    {
        public SimpleCompositeKeyEntityRepository(IDatabaseFactory<YourCustomDataContext> dbFactory) : base(dbFactory.GetNewDbContext())
        {
        }
    }
}
