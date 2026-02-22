using System.Collections.Generic;

namespace EntityFramework.Repository.Core.Interfaces
{
    public interface ICreateFunctions<T>
            where T : class
    {
        T Add(T entity);
        IEnumerable<T> Add(IEnumerable<T> entities);
    }
}
