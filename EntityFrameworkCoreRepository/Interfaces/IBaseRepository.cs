using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Repository.Core.Interfaces
{
    public interface IBaseRepository<C, T> :
        IDisposable
        , IReadFunctions<T>
        , IUpdateFunctions<T>
        , IDeleteFunctions<T>
        , ICreateFunctions<T>
        , ISaveFunctions<T>
            where T : class
            where C : DbContext
    {
        int Count();
        Task<int> CountAsync();
        bool Exists(Expression<Func<T, bool>> predicate);
        void Reload(T entity);
        Task ReloadAsync(T entity);
    }
}
