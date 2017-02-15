using EntityFramework.Auditing;
using System.Data.Common;
using System.Data.Entity;

namespace EntityFrameworkAuditableRepository6.Base
{
    public class BaseContext<TContext> : AuditDbContext
            where TContext : AuditDbContext
    {
        static BaseContext()
        {
            //Database.SetInitializer<TContext>(null);
        }

        protected BaseContext(DbConnection dbConnection) : base(dbConnection, true) { Database.SetInitializer<TContext>(null); }

        protected BaseContext(string connectionString) : base(connectionString) { Database.SetInitializer<TContext>(null); }

        protected BaseContext(string connectionString, IDatabaseInitializer<TContext> initializerStrategy) : base(connectionString)
        {
            Database.SetInitializer<TContext>(initializerStrategy);
        }

        protected BaseContext(DbConnection dbConnection, IDatabaseInitializer<TContext> initializerStrategy) : base(dbConnection, true)
        {
            Database.SetInitializer<TContext>(initializerStrategy);
        }
    }
}
