using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.SharedRepository
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
