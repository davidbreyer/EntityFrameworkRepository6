using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Repository6.Interfaces
{
    public interface IAuditSaveFunctions<T>
            where T : class
    {
        Task<int> SaveAsync(string userName);
        int Save(string userName);
    }
}
