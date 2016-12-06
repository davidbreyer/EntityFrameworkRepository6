using EntityFrameworkRepository6.Base;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistentLayer.Entities;
using PersistentLayer.Repositories;
using System;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;

namespace EntityFrameworkRepository6Tests
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
        
        [TestMethod]
        public void UpdateTestMethod()
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

        [TestMethod]
        public void UpdateTestMethodWithIgnoreFieldFeature()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            repository.AddUpdateIgnoreField("Name");

            var itemToUpdate = repository.Find(2);
            itemToUpdate.Name = "Updated Name";
            repository.Update(itemToUpdate, itemToUpdate.Id);
            repository.Save();

            var actual = repository.Find(2);

            Assert.AreNotEqual("Updated Name", actual.Name);

            repository.Dispose();
        }

        [TestMethod]
        public void UpdateByTestMethod()
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

        [TestMethod]
        [ExpectedException(typeof(System.Exception))]
        public void UpdateByTestMethodIncorrectId()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();

            var itemToUpdate = repository.Find(2);
            itemToUpdate.Name = "Updated Name 2";
            repository.Update(itemToUpdate, x => x.Id == 55);
        }
    }
}
