using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Repository.Core
{
    public interface IDatabaseFactory<C>
    {
        C GetNewDbContext();
    }

    public class DatabaseFactory<C> : IDatabaseFactory<C>
        where C : DbContext
    {
        protected string DbConnectionString { get; set; }

        public DatabaseFactory(string connectionString)
        {
            DbConnectionString = connectionString;
        }

        public virtual C GetNewDbContext()
        {
            return (C)Activator.CreateInstance(typeof(C), DbConnectionString)!;
        }
    }
}
