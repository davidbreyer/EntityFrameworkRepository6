using EntityFramework.SharedRepository;
using EntityFrameworkRepository6.Base;
using PersistentLayer.Contexts;
using PersistentLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistentLayer.Repositories
{
    public interface ISimpleDataEntityRepository : IBaseRepository<YourCustomDataContext, SimpleDataEntity>
    {
    }

    public class SimpleDataEntityRepository : BaseRepository<YourCustomDataContext, SimpleDataEntity>, ISimpleDataEntityRepository
    {
        public SimpleDataEntityRepository(IDatabaseFactory<YourCustomDataContext> dbFactory) : base(dbFactory.GetNewDbContext())
        {
            //Context = dbFactory.GetNewDbContext();
        }
    }
}
