using EntityFramework.Auditing;
using EntityFramework.Repository6;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistentLayer.Auditable.Entities;
using PersistentLayer.Auditable.Repositories;
using System;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;

namespace EntityFramework.Auditable.Repository6.Tests
{
    [TestClass]
    public class RepositoryUpdateTests
    {
        UnityContainer LocalIoCContainer { get; set; }

        [TestInitialize]
        public void Setup()
        {
            Action<string> logSetup = message => Debug.WriteLine(message);

            LocalIoCContainer = new UnityContainer();
            LocalIoCContainer.RegisterType<ISimpleDataEntityRepository, SimpleDataEntityRepository>(new HierarchicalLifetimeManager());
            LocalIoCContainer.RegisterType<ISimpleCompositeKeyEntityRepository, SimpleCompositeKeyEntityRepository>(new HierarchicalLifetimeManager());



            //AuditDbContext.RegisterAuditType(typeof(SimpleDataEntity), typeof(SimpleDataAuditEntity));

            //Use a physical database file
            //Direct connection string
            //var connectionString = @"Data Source = (LocalDb)\MSSQLLocalDB; Database = TestDatabase; Integrated Security = True; Pooling = false; MultipleActiveResultSets = true";
            //or
            //Computed connection
            //var connectionString = @"TestDatabase";
            //LocalIoCContainer.RegisterType(typeof(IDatabaseFactory<>), typeof(DatabaseFactory<>)
            //    , new HierarchicalLifetimeManager()
            //    , new InjectionConstructor(connectionString)
            //    , new InjectionProperty("Logging", logSetup));

            //Use an nmemory database
            LocalIoCContainer.RegisterType(typeof(IDatabaseFactory<>), typeof(TestExampleDatabaseFactory)
                , new HierarchicalLifetimeManager()
                , new InjectionProperty("Logging", logSetup));
        }

        [TestCategory("AuditRepository")]
        [TestMethod]
        public void UpdateTestMethodAudit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            
            var itemToUpdate = repository.Find(2);
            itemToUpdate.Name = "Updated Name";
            repository.Update(itemToUpdate, itemToUpdate.Id);
            repository.Save();

            var actual = repository.Find(2);
            
            Assert.AreEqual("Updated Name", actual.Name);

            repository.Dispose();
        }

        [TestCategory("AuditRepository")]
        [TestMethod]
        public void UpdateTestMethodWithIgnoreFieldFeatureAudit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            repository.AddUpdateIgnoreField("Name");

            var itemToUpdate = repository.Find(2);
            itemToUpdate.Name = "Updated Name";
            repository.Update(itemToUpdate, itemToUpdate.Id);
            repository.Save("UserName1");

            var actual = repository.Find(2);

            Assert.AreNotEqual("Updated Name", actual.Name);

            repository.Dispose();
        }

        [TestCategory("AuditRepository")]
        [TestMethod]
        public void UpdateByTestMethodAudit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();

            var itemToUpdate = repository.Find(2);
            itemToUpdate.Name = "Updated Name 2";
            repository.Update(itemToUpdate, x => x.Id == 2);
            repository.Save();

            var actual = repository.Find(2);

            Assert.AreEqual("Updated Name 2", actual.Name);

            repository.Dispose();
        }

        [TestCategory("AuditRepository")]
        [TestMethod]
        public void UpdateByAuditTestMethod()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            Assert.IsTrue(AuditDbContext.AuditEnabled);

            AuditDbContext.RegisterAuditType(typeof(SimpleDataEntity), typeof(SimpleDataEntityAudit));

            var itemToUpdate = repository.Find(2);
            itemToUpdate.Name = "Updated Name 3";
            repository.Update(itemToUpdate, x => x.Id == 2);

            repository.Save("UserName2");

            var actual = repository.Find(2);

            var auditactual = repository.GetExistingContext().SimpleDataEntityAudits.Where(x => x.AuditSourceId == 2).FirstOrDefault();

            Assert.AreEqual("Updated Name 3", actual.Name);
            Assert.AreEqual("Test 2", auditactual?.Name);
            Assert.AreEqual("UserName2", auditactual?.AuditUser);

            repository.Dispose();
        }

        [TestCategory("AuditRepository")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UpdateByTestMethodIncorrectIdAudit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();

            var itemToUpdate = repository.Find(2);
            itemToUpdate.Name = "Updated Name 2";
            repository.Update(itemToUpdate, x => x.Id == 55);
        }
    }
}
