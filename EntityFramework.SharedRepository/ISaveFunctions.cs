using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.SharedRepository
{
    public interface ISaveFunctions<T> : IDisposable
            where T : class
    {
        Task<int> SaveAsync();
        int Save();
    }
}
