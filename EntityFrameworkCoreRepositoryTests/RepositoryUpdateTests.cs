using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistentLayerCore.Repositories;
using System;

namespace EntityFramework.Repository.Core.Tests
{
    [TestClass]
    public class RepositoryUpdateTests
    {
        TestExampleDatabaseFactory DatabaseFactory { get; set; } = null!;

        [TestInitialize]
        public void Setup()
        {
            DatabaseFactory = new TestExampleDatabaseFactory();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void UpdateTestMethod()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);

            var itemToUpdate = repository.Find(2)!;
            itemToUpdate.Name = "Updated Name";
            repository.Update(itemToUpdate, itemToUpdate.Id);
            repository.Save();

            var actual = repository.Find(2);

            Assert.AreEqual("Updated Name", actual!.Name);

            repository.Dispose();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void UpdateTestMethodWithIgnoreFieldFeature()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);
            repository.AddUpdateIgnoreField("Name");

            var itemToUpdate = repository.Find(2)!;
            itemToUpdate.Name = "Updated Name";
            repository.Update(itemToUpdate, itemToUpdate.Id);
            repository.Save();

            var actual = repository.Find(2);

            Assert.AreNotEqual("Updated Name", actual!.Name);

            repository.Dispose();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void UpdateByTestMethod()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);

            var itemToUpdate = repository.Find(2)!;
            itemToUpdate.Name = "Updated Name 2";
            repository.Update(itemToUpdate, x => x.Id == 2);
            repository.Save();

            var actual = repository.Find(2);

            Assert.AreEqual("Updated Name 2", actual!.Name);

            repository.Dispose();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UpdateByTestMethodIncorrectId()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);

            var itemToUpdate = repository.Find(2)!;
            itemToUpdate.Name = "Updated Name 2";
            repository.Update(itemToUpdate, x => x.Id == 55);
        }
    }
}
