using PersistentLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseInitialization
{
    public class DatabaseInitializer : MigrateDatabaseToLatestVersion<TestDatabase, Migrations.Configuration>
    {
        protected void Seed(TestDatabase context)
        {
            var mydata1 = new SimpleDataEntity { Name = "Hello Test 2" };
            var mydata2 = new SimpleDataEntity { Name = "Hello Test Again 2" };
            //var anotherData = new AnotherData2 { Name = "Composite 1" };

            context.SimpleDataEntities.Add(mydata1);
            context.SimpleDataEntities.Add(mydata2);
            //context.AnotherDataElements.Add(anotherData);
            context.SaveChanges();
        }
    }
}
