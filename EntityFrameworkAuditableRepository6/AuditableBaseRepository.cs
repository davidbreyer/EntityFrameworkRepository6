using EntityFramework.Auditing;
using EntityFramework.Repository6;
using EntityFramework.Repository6.Interfaces;
using System;
using System.Threading.Tasks;

namespace EntityFramework.Auditable.Repository6
{
    public abstract class AuditableBaseRepository<C, T> :
        BaseRepository<C, T>
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
