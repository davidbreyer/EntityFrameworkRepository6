using EntityFramework.Auditing;
using System.Data.Common;
using System.Data.Entity;

namespace EntityFramework.Auditable.Repository6
{
    public class AuditableBaseContext<TContext> : AuditDbContext
            where TContext : AuditDbContext
    {
        static AuditableBaseContext() { }
    
        protected AuditableBaseContext(DbConnection dbConnection) : base(dbConnection, true) { Database.SetInitializer<TContext>(null); }

        protected AuditableBaseContext(DbConnection dbConnection, bool contextOwnsConnection) : base(dbConnection, contextOwnsConnection) { Database.SetInitializer<TContext>(null); }

        protected AuditableBaseContext(string connectionString) : base(connectionString) { Database.SetInitializer<TContext>(null); }

        protected AuditableBaseContext(string connectionString, IDatabaseInitializer<TContext> initializerStrategy) : base(connectionString) { Database.SetInitializer<TContext>(initializerStrategy); }

        protected AuditableBaseContext(DbConnection dbConnection, IDatabaseInitializer<TContext> initializerStrategy) : base(dbConnection, true) { Database.SetInitializer<TContext>(initializerStrategy); }

        protected AuditableBaseContext(DbConnection dbConnection, IDatabaseInitializer<TContext> initializerStrategy, bool contextOwnsConnection) : base(dbConnection, contextOwnsConnection) { Database.SetInitializer<TContext>(initializerStrategy); }
    }
}
