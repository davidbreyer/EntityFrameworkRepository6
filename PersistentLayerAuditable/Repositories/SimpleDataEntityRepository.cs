using System;
using EntityFramework.SharedRepository;
using EntityFrameworkAuditableRepository6.Base;
using PersistentLayerAuditable.Contexts;
using PersistentLayerAuditable.Entities;
using EntityFrameworkRepository6.Base;

namespace PersistentLayerAuditable.Repositories
{
    public interface ISimpleDataEntityRepository : IBaseRepository<YourCustomDataContext, SimpleDataEntity>, IAuditSaveFunctions<SimpleDataEntity>
    {
        YourCustomDataContext GetExistingContext();
    }

    public class SimpleDataEntityRepository : EntityFrameworkAuditableRepository6.AuditableBaseRepository<YourCustomDataContext, SimpleDataEntity>, ISimpleDataEntityRepository
    {
        public SimpleDataEntityRepository(IDatabaseFactory<YourCustomDataContext> dbFactory) : base( dbFactory.GetNewDbContext() )
        {
            //Context = dbFactory.GetNewDbContext();
        }

        public YourCustomDataContext GetExistingContext()
        {
            return Context;
        }
    }
}
