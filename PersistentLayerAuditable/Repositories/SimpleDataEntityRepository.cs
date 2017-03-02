using System;
using PersistentLayer.Auditable.Contexts;
using PersistentLayer.Auditable.Entities;
using EntityFramework.Repository6.Interfaces;
using EntityFramework.Auditable.Repository6;
using EntityFramework.Repository6;

namespace PersistentLayer.Auditable.Repositories
{
    public interface ISimpleDataEntityRepository : IBaseRepository<YourCustomDataContext, SimpleDataEntity>, IAuditSaveFunctions<SimpleDataEntity>
    {
        YourCustomDataContext GetExistingContext();
    }

    public class SimpleDataEntityRepository : AuditableBaseRepository<YourCustomDataContext, SimpleDataEntity>, ISimpleDataEntityRepository
    {
        public SimpleDataEntityRepository(IDatabaseFactory<YourCustomDataContext> dbFactory) : base( dbFactory.GetNewDbContext() )
        {
            
        }

        public YourCustomDataContext GetExistingContext()
        {
            return Context;
        }
    }
}
