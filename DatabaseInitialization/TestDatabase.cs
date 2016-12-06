using EntityFrameworkRepository6.Base;
using PersistentLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseInitialization
{
    public class TestDatabase : BaseContext<TestDatabase>
    {
        public DbSet<SimpleDataEntity> SimpleDataEntities { get; set; }
        public DbSet<SimpleCompositeKeyEntity> SimpleCompositeKeyEntities { get; set; }

        public TestDatabase() : base("TestDatabase") { }

        protected void Seed(TestDatabase context)
        {
            var simpleDataEntity1 = new SimpleDataEntity { Name = "Hello Test 2" };
            var simpleDataEntity2 = new SimpleDataEntity { Name = "Hello Test Again 2" };
            var simpleCompositeKeyEntity = new SimpleCompositeKeyEntity { Name = "Composite 1" };

            context.SimpleDataEntities.Add(simpleDataEntity1);
            context.SimpleDataEntities.Add(simpleDataEntity2);
            context.SimpleCompositeKeyEntities.Add(simpleCompositeKeyEntity);
            context.SaveChanges();
        }
    }
}
