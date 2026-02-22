using System.Collections.Generic;

namespace EntityFramework.Repository.Core.Interfaces
{
    public interface IDeleteFunctions<T>
            where T : class
    {
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
    }
}
