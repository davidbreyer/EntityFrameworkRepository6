using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Repository6.Interfaces
{
    public interface IDeleteFunctions<T>
            where T : class
    {
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
    }
}
