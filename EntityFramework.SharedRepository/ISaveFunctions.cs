using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Repository6.Interfaces
{
    public interface ISaveFunctions<T>
            where T : class
    {
        Task<int> SaveAsync();
        int Save();
    }
}
