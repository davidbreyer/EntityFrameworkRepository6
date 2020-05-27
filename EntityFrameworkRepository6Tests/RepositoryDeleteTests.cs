using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistentLayer.Repositories;
using System;
using System.Diagnostics;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace EntityFramework.Repository6.Tests
{
    [TestClass]
    public class RepositoryDeleteTests
    {
        UnityContainer LocalIoCContainer { get; set; }

        [TestInitialize]
        public void Setup()
        {
            Action<string> logSetup = message => Debug.WriteLine(message);

            LocalIoCContainer = new UnityContainer();
            LocalIoCContainer.RegisterType<ISimpleDataEntityRepository, SimpleDataEntityRepository>(new HierarchicalLifetimeManager());
            LocalIoCContainer.RegisterType<ISimpleCompositeKeyEntityRepository, SimpleCompositeKeyEntityRepository>(new HierarchicalLifetimeManager());

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

        [TestCategory("StandardRepository")]
        [TestMethod]
        public void DeleteTestMethod()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();

            var actual1 = repository.Count();

            var itemToDelete = repository.Find(1);

            repository.Delete(itemToDelete);

            repository.Save();

            var actual2 = repository.Count();

            Assert.AreEqual(7, actual1);
            Assert.AreEqual(6, actual2);

            repository.Dispose();
        }

        [TestCategory("StandardRepository")]
        [TestMethod]
        public void DeleteMultipleTestMethod()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();

            var actual1 = repository.Count();

            var itemsToDelete = repository.FindBy(x => x.Id == 1 || x.Id == 2);

            repository.Delete(itemsToDelete);

            repository.Save();

            var actual2 = repository.Count();

            Assert.AreEqual(7, actual1);
            Assert.AreEqual(5, actual2);

            repository.Dispose();
        }
    }
}
