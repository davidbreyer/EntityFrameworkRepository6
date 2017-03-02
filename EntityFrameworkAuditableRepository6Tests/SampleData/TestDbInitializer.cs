using PersistentLayer.Auditable.Contexts;
using PersistentLayer.Auditable.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Auditable.Repository6.Tests
{
    public class TestDbInitializer : DropCreateDatabaseAlways<YourCustomDataContext>
    {
        protected override void Seed(YourCustomDataContext context)
        {
            var mydata1 = new SimpleDataEntity { Name = "Hello Test 2" };
            var mydata2 = new SimpleDataEntity { Name = "Hello Test Again 2" };

            context.SimpleDataEntities.Add(mydata1);
            context.SimpleDataEntities.Add(mydata2);
            var result = context.SaveChangesAsync().Result;
        }
    }
}
