using System;
using System.Linq.Expressions;

namespace EntityFramework.Repository.Core.Interfaces
{
    public interface IUpdateFunctions<T>
            where T : class
    {
        void Update(T entity, int id);
        void Update(T entity, Expression<Func<T, bool>> predicate);
        void Update(Delta<T> delta, params object[] ids);
        void Update(Delta<T> delta, Expression<Func<T, bool>> predicate);
        void AddUpdateIgnoreField(string fieldName);
        void AddUpdateIgnoreField(Expression<Func<T, bool>> fieldName);
    }
}
