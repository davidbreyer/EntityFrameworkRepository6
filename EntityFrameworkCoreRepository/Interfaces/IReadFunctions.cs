using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EntityFramework.Repository.Core.Interfaces
{
    public interface IReadFunctions<T>
            where T : class
    {
        IQueryable<T> GetAll();
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        IQueryable<T> FindByReadOnly(Expression<Func<T, bool>> predicate);
        T? Find(int id);
        T? Find(params object[] ids);
        Task<T?> FindAsync(int id);
        Task<T?> FindAsync(params object[] ids);
    }
}
