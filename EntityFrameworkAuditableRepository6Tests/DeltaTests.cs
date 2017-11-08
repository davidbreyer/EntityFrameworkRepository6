using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using System.Linq;
using System.Data.Entity.Infrastructure;
using PersistentLayer.Auditable.Repositories;
using EntityFramework.Auditable.Repository6.Tests;
using PersistentLayer.Auditable.Entities;
using EntityFramework.Repository6;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace EntityFramework.Repository6.Tests
{
    [TestClass]
    public class DeltaTests
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

        [TestCategory("AuditRepository")]
        [TestMethod]
        public void DeltaTest1Audit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            var newItem = new SimpleDataEntity { Name = "Delta Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            var delta1 = new Delta<SimpleDataEntity>();
            delta1.SetValue("Name", "Delta Change Test");

            repository.Update(delta1, newItem.Id);

            repository.Save();

            var updatedValue = repository.Find(actual.Id);
            Assert.AreEqual("Delta Change Test", updatedValue.Name);
        }

        [TestCategory("AuditRepository")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DeltaTestWrongDataType1Audit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            var newItem = new SimpleDataEntity { Name = "Delta Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            var delta1 = new Delta<SimpleDataEntity>();
            delta1.SetValue("Name", 1);
        }

        [TestCategory("AuditRepository")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DeltaTestInvalidFieldName1Audit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            var newItem = new SimpleDataEntity { Name = "Delta Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            var delta1 = new Delta<SimpleDataEntity>();
            delta1.SetValue("NameNotRight", "Right data type");
        }


        [TestCategory("AuditRepository")]
        [TestMethod]
        public void DeltaTest2Audit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            var newItem = new SimpleDataEntity { Name = "Delta Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            var delta2 = new Delta<SimpleDataEntity>();
            delta2.SetValue("Name", "Delta Change Test 2");

            repository.Update(delta2, e => e.Id == newItem.Id);

            repository.Save();

            var updatedValue = repository.Find(actual.Id);
            Assert.AreEqual("Delta Change Test 2", updatedValue.Name);
        }

        [TestCategory("AuditRepository")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeltaTestFailAudit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            var newItem = new SimpleDataEntity { Name = "Delta Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            var delta1 = new Delta<SimpleDataEntity>();
            delta1.SetValue("Name", "Delta Change Test");

            repository.Update(delta1, 55);
        }

        [TestCategory("AuditRepository")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeltaTestFailWithPredicateAudit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            var newItem = new SimpleDataEntity { Name = "Delta Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            var delta1 = new Delta<SimpleDataEntity>();
            delta1.SetValue("Name", "Delta Change Test");

            repository.Update(delta1, x=>x.Id == 55);
        }
    }
}
