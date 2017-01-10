using EntityFramework.Auditing;
using System;
using System.Data.Entity;

namespace EntityFrameworkAuditableRepository6.Base
{
    public interface IDatabaseFactory<C>
    {
        C GetNewDbContext();
        Action<string> Logging { get; set; }
    }

    public class DatabaseFactory<C> : IDatabaseFactory<C>
        where C : AuditDbContext
    {
        public Action<string> Logging { get; set; }

        protected string dbConnectionString { get; set; }

        public DatabaseFactory(string connectionString)
        {
            dbConnectionString = connectionString;
        }
        
        public virtual C GetNewDbContext()
        {
            var newContext = (C)Activator.CreateInstance(typeof(C), dbConnectionString);

            if (Logging != null) { newContext.Database.Log = Logging; }
            
            return newContext;
        }
    }
}
