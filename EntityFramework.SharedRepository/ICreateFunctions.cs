using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Repository6.Interfaces
{
    public interface ICreateFunctions<T>
            where T : class
    {
        T Add(T entity);
        IEnumerable<T> Add(IEnumerable<T> entities);
    }
}
