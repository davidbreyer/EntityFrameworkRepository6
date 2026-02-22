using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistentLayerCore.Repositories;
using System.Linq;

namespace EntityFramework.Repository.Core.Tests
{
    [TestClass]
    public class RepositoryReadTests
    {
        TestExampleDatabaseFactory DatabaseFactory { get; set; } = null!;

        [TestInitialize]
        public void Setup()
        {
            DatabaseFactory = new TestExampleDatabaseFactory();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void GetAllFromSimpleDataEntitiesTable()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);

            var actual = repository.GetAll();
            Assert.AreEqual(7, actual.Count());

            repository.Dispose();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void CountOfSimpleDataEntitiesTable()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);

            var actual = repository.Count();
            Assert.AreEqual(7, actual);

            repository.Dispose();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void FindTestMethod1()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);

            var actual = repository.Find(1);
            Assert.AreEqual("Test 1", actual!.Name);

            repository.Dispose();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void SelectTestMethod()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);

            var actual = repository.FindBy(x => x.Id == 2).FirstOrDefault();
            Assert.AreEqual("Test 2", actual!.Name);

            repository.Dispose();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void FindAsyncTest()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);

            var actual = repository.FindAsync(2);
            Assert.AreEqual("Test 2", actual.Result!.Name);

            repository.Dispose();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void FindAsyncMultipleParamsTest()
        {
            var repository = new SimpleCompositeKeyEntityRepository(DatabaseFactory);

            var actual = repository.FindAsync(1, "Another Key");
            Assert.AreEqual("Another Key", actual.Result!.Name);

            repository.Dispose();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void FindByReadOnlyTest()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);

            var actual = repository.FindByReadOnly(x => x.Id == 1).SingleOrDefault();
            Assert.AreEqual("Test 1", actual!.Name);

            repository.Dispose();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void ExistsTest()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);

            var actual = repository.Exists(x => x.Id == 1);
            Assert.IsTrue(actual);

            repository.Dispose();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void FindAndReloadAsyncTest()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);

            var actualEntity = repository.FindAsync(2).Result!;
            Assert.AreEqual("Test 2", actualEntity.Name);

            actualEntity.Name = "Test 10";
            Assert.AreEqual("Test 10", actualEntity.Name);

            repository.ReloadAsync(actualEntity).Wait();

            Assert.AreEqual("Test 2", actualEntity.Name);
            repository.Dispose();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void FindAndReloadTest()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);

            var actualEntity = repository.Find(2)!;
            Assert.AreEqual("Test 2", actualEntity.Name);

            actualEntity.Name = "Test 10";
            Assert.AreEqual("Test 10", actualEntity.Name);

            repository.Reload(actualEntity);

            Assert.AreEqual("Test 2", actualEntity.Name);
            repository.Dispose();
        }
    }
}
