using PersistentLayer.Auditable.Contexts;
using PersistentLayer.Auditable.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistentLayer.Auditable.Initializers
{
    public class MyDbInitializer : DropCreateDatabaseIfModelChanges<YourCustomDataContext>
    {
        protected override void Seed(YourCustomDataContext context)
        {
            var mydata1 = new SimpleDataEntity { Name = "Hello Test" };
            var mydata2 = new SimpleDataEntity { Name = "Hello Test Again" };

            context.SimpleDataEntities.Add(mydata1);
            context.SimpleDataEntities.Add(mydata2);
            context.SaveChanges();
        }
    }
}
