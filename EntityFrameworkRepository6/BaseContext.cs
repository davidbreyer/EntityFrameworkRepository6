using System.Data.Common;
using System.Data.Entity;

namespace EntityFramework.Repository6
{
    public class BaseContext<TContext> : DbContext
            where TContext : DbContext
    {
        static BaseContext() { }

        protected BaseContext(DbConnection dbConnection) : base(dbConnection, true) { Database.SetInitializer<TContext>(null); }

        protected BaseContext(DbConnection dbConnection, bool contextOwnsConnection) : base(dbConnection, contextOwnsConnection) { Database.SetInitializer<TContext>(null); }

        protected BaseContext(string connectionString) : base(connectionString) { Database.SetInitializer<TContext>(null); }

        protected BaseContext(string connectionString, IDatabaseInitializer<TContext> initializerStrategy) : base(connectionString) { Database.SetInitializer<TContext>(initializerStrategy); }

        protected BaseContext(DbConnection dbConnection, IDatabaseInitializer<TContext> initializerStrategy) : base(dbConnection, true) { Database.SetInitializer<TContext>(initializerStrategy); }

        protected BaseContext(DbConnection dbConnection, IDatabaseInitializer<TContext> initializerStrategy, bool contextOwnsConnection) : base(dbConnection, contextOwnsConnection) { Database.SetInitializer<TContext>(initializerStrategy); }
    }
}
