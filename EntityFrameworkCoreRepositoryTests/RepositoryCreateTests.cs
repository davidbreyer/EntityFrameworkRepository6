using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistentLayerCore.Entities;
using PersistentLayerCore.Repositories;
using System.Collections.Generic;

namespace EntityFramework.Repository.Core.Tests
{
    [TestClass]
    public class RepositoryCreateTests
    {
        TestExampleDatabaseFactory DatabaseFactory { get; set; } = null!;

        [TestInitialize]
        public void Setup()
        {
            DatabaseFactory = new TestExampleDatabaseFactory();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void InsertTestMethod()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);
            var newItem = new SimpleDataEntity { Name = "My Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            Assert.AreNotEqual(0, actual.Id);

            var actual2 = repository.Find(actual.Id);

            Assert.AreEqual("My Test", actual2!.Name);

            repository.Dispose();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void InsertMultipleTest()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);

            var actual1 = repository.Count();

            var newItem1 = new SimpleDataEntity { Name = "Multiple Item 1" };
            var newItem2 = new SimpleDataEntity { Name = "Multiple Item 2" };
            var newItem3 = new SimpleDataEntity { Name = "Multiple Item 3" };
            repository.Add(new List<SimpleDataEntity> { newItem1, newItem2, newItem3 });
            repository.Save();

            var actual2 = repository.Count();

            Assert.AreEqual(7, actual1);
            Assert.AreEqual(10, actual2);

            repository.Dispose();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void InsertIntoCompositeKeyTableMethod()
        {
            var repository = new SimpleCompositeKeyEntityRepository(DatabaseFactory);
            var newItem = new SimpleCompositeKeyEntity { Id = 2, Name = "Composite Test" };
            var actual = repository.Add(newItem);
            var result = repository.Save();

            Assert.AreNotEqual(0, actual.Id);

            var actual2 = repository.Find(actual.Id, actual.Name);

            Assert.AreEqual("Composite Test", actual2!.Name);

            repository.Dispose();
        }
    }
}
