﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using PersistentLayerAuditable.Repositories;
using System.Linq;
using PersistentLayerAuditable.Entities;
using System.Data.Entity.Infrastructure;
using EntityFrameworkAuditableRepository6.Base;
using EntityFramework.Auditing;

namespace EntityFrameworkAuditableRepository6Tests
{
    [TestClass]
    public class RepositoryCreateTests
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
        public void InsertTestMethodAudit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            var newItem = new SimpleDataEntity { Name = "My Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            Assert.AreNotEqual(0, actual.Id);

            var actual2 = repository.Find(actual.Id);

            Assert.AreEqual("My Test", actual2.Name);

            repository.Dispose();
        }

        [TestCategory("AuditRepository")]
        [TestMethod]
        public void InsertTestMethod2Audit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            var newItem = new SimpleDataEntity { Name = "My Test 200" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            Assert.AreNotEqual(0, actual.Id);

            var actual2 = repository.Find(actual.Id);

            Assert.AreEqual("My Test 200", actual2.Name);

            repository.Dispose();
        }

        [TestCategory("AuditRepository")]
        [TestMethod]
        public void InsertTestMethod3Audit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleDataEntityRepository>();
            var newItem = new SimpleDataEntity { Name = "My Test 300" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            Assert.AreNotEqual(0, actual.Id);

            var actual2 = repository.Find(actual.Id);

            Assert.AreEqual("My Test 300", actual2.Name);

            repository.Dispose();
        }

        [TestCategory("AuditRepository")]
        [TestMethod]
        public void InsertIntoCompositeKeyTableMethodAudit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleCompositeKeyEntityRepository>();
            var newItem = new SimpleCompositeKeyEntity { Id = 1, Name = "Composite Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            Assert.AreNotEqual(0, actual.Id);

            var actual2 = repository.Find(actual.Id, actual.Name);

            var saveAgain = repository.Add(new SimpleCompositeKeyEntity { Id = 2, Name = "Composite Test" });
            var result2 = repository.Save();

            Assert.AreNotEqual(actual.Id, saveAgain.Id);

            Assert.AreEqual("Composite Test", actual2.Name);

            repository.Dispose();
        }

        [TestCategory("AuditRepository")]
        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void InsertDuplicateIntoCompositeKeyTableMethodAudit()
        {
            var repository = LocalIoCContainer.Resolve<ISimpleCompositeKeyEntityRepository>();
            var newItem = new SimpleCompositeKeyEntity { Id = 1, Name = "Composite Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            Assert.AreNotEqual(0, actual.Id);

            var actual2 = repository.Find(actual.Id, actual.Name);

            var saveAgain = repository.Add(new SimpleCompositeKeyEntity { Id = 1, Name = "Composite Test" });
            var result2 = repository.Save();
        }
    }
}