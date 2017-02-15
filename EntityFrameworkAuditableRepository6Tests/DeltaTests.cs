using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using System.Linq;
using System.Data.Entity.Infrastructure;
using PersistentLayerAuditable.Repositories;
using EntityFrameworkAuditableRepository6.Base;
using EntityFrameworkAuditableRepository6Tests;
using PersistentLayerAuditable.Entities;
using EntityFramework.SharedRepository;
using EntityFrameworkRepository6.Base;

namespace EntityFrameworkRepository6Tests
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
    }
}
