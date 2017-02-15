using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.SharedRepository
{
    public interface ICreateFunctions<T> : IDisposable
            where T : class
    {
        T Add(T entity);
        IEnumerable<T> Add(IEnumerable<T> entities);
    }
}
