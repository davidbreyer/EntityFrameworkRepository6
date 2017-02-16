using PersistentLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.Entity.ModelConfiguration.Conventions;
using PersistentLayer.Initializers;
using EntityFramework.Repository6;

namespace PersistentLayer.Contexts
{
    public class YourCustomDataContext : BaseContext<YourCustomDataContext>
    {
        public DbSet<SimpleDataEntity> SimpleDataEntities { get; set; }
        public DbSet<SimpleCompositeKeyEntity> SimpleCompositeKeyEntities { get; set; }


        public YourCustomDataContext(string connectionString) : base(connectionString) { }
        public YourCustomDataContext(DbConnection dbConnection) : base(dbConnection) { }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //}
    }
}
