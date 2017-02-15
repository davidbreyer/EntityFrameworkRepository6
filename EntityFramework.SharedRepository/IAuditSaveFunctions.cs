using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.SharedRepository
{
    public interface IAuditSaveFunctions<T> : IDisposable
            where T : class
    {
        Task<int> SaveAsync(string userName);
        int Save(string userName);
    }
}
