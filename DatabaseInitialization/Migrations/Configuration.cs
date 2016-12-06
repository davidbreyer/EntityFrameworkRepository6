namespace DatabaseInitialization.Migrations
{
    using PersistentLayer.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<DatabaseInitialization.TestDatabase>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DatabaseInitialization.TestDatabase context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var mydata1 = new SimpleDataEntity { Name = "Hello Test 2" };
            var mydata2 = new SimpleDataEntity { Name = "Hello Test Again 2" };

            context.SimpleDataEntities.AddOrUpdate(mydata1);
            context.SimpleDataEntities.AddOrUpdate(mydata2);
            context.SaveChanges();
        }
    }
}
