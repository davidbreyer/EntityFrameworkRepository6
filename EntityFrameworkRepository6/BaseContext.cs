using System.Data.Common;
using System.Data.Entity;

namespace EntityFrameworkRepository6.Base
{
    public class BaseContext<TContext> : DbContext
            where TContext : DbContext
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
