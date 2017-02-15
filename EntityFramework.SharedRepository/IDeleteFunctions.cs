using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.SharedRepository
{
    public interface IDeleteFunctions<T> : IDisposable
            where T : class
    {
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
    }
}
