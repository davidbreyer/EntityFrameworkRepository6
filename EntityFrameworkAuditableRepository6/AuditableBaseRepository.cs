using EntityFramework.Auditing;
using EntityFramework.SharedRepository;
using System;
using System.Threading.Tasks;

namespace EntityFrameworkAuditableRepository6
{
    public abstract class AuditableBaseRepository<C, T> :
        EntityFrameworkRepository6.Base.BaseRepository<C, T>
        , IBaseRepository<C, T>
        , IDisposable
        , IAuditSaveFunctions<T>
        where T : class
        where C : AuditDbContext
    {
        protected AuditableBaseRepository(C context) : base(context)
        {
        }

        public async virtual Task<int> SaveAsync(string userName)
        {
            return await Context.SaveChangesAsync(userName);
        }

        public virtual int Save(string userName)
        {
            return Context.SaveChanges(userName);
        }
    }
}
