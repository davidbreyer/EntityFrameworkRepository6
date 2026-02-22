using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistentLayerCore.Entities;
using PersistentLayerCore.Repositories;
using System;

namespace EntityFramework.Repository.Core.Tests
{
    [TestClass]
    public class DeltaTests
    {
        TestExampleDatabaseFactory DatabaseFactory { get; set; } = null!;

        [TestInitialize]
        public void Setup()
        {
            DatabaseFactory = new TestExampleDatabaseFactory();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void DeltaTest1()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);
            var newItem = new SimpleDataEntity { Name = "Delta Test" };
            var actual = repository.Add(newItem);
            repository.Save();

            var delta1 = new Delta<SimpleDataEntity>();
            delta1.SetValue("Name", "Delta Change Test");

            repository.Update(delta1, newItem.Id);

            repository.Save();

            var updatedValue = repository.Find(actual.Id);
            Assert.AreEqual("Delta Change Test", updatedValue!.Name);
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DeltaTestWrongDataType1()
        {
            var delta1 = new Delta<SimpleDataEntity>();
            delta1.SetValue("Name", 1);
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DeltaTestInvalidFieldName1()
        {
            var delta1 = new Delta<SimpleDataEntity>();
            delta1.SetValue("NameNotRight", "Right data type");
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void DeltaTestWithPredicate()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);
            var newItem = new SimpleDataEntity { Name = "Delta Test" };
            var actual = repository.Add(newItem);
            repository.Save();

            var delta1 = new Delta<SimpleDataEntity>();
            delta1.SetValue("Name", "Delta Change Test");

            repository.Update(delta1, x => x.Id == newItem.Id);

            repository.Save();

            var updatedValue = repository.Find(actual.Id);
            Assert.AreEqual("Delta Change Test", updatedValue!.Name);
        }
    }
}
