using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using PersistentLayerAuditable.Repositories;
using System.Linq;
using EntityFrameworkAuditableRepository6.Base;
using EntityFramework.Auditing;
using PersistentLayerAuditable.Entities;

namespace EntityFrameworkAuditableRepository6Tests
{
    [TestClass]
    public class RepositoryReadTests
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
            //var connectionString = @"Data Source = (LocalDb)\MSSQLLocalDB; Database = TestDatabase; Integrated Security = True; Pooling = false; MultipleActiveResultSets = true";
            //or
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
        public void GetAllFromSimpleDataEntitiesTableAudit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();

            var actual = repository.GetAll();
            Assert.AreEqual(7, actual.Count());

            repository.Dispose();
        }

        [TestCategory("AuditRepository")]
        [TestMethod]
        public void FindTestMethod1Audit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();

            var actual = repository.Find(1);
            Assert.AreEqual("Test 1", actual.Name);

            repository.Dispose();
        }

        [TestCategory("AuditRepository")]
        [TestMethod]
        public void SelectTestMethodAudit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();

            var actual = repository.FindBy(x => x.Id == 2).FirstOrDefault();
            Assert.AreEqual("Test 2", actual.Name);

            repository.Dispose();
        }

        [TestCategory("AuditRepository")]
        [TestMethod]
        public void FindAsyncTestAudit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();

            var actual = repository.FindAsync(2);
            Assert.AreEqual("Test 2", actual.Result.Name);

            repository.Dispose();
        }

        [TestCategory("AuditRepository")]
        [TestMethod]
        public void FindAsyncMultipleParamsTestAudit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleCompositeKeyEntityRepository>();

            var actual = repository.FindAsync(1, "Another Key");
            Assert.AreEqual("Another Key", actual.Result.Name);

            repository.Dispose();
        }
    }
}
