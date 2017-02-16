using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EntityFramework.Repository6.Interfaces
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
        void ReloadAsync(T entity);
    }
}
