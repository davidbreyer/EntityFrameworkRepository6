using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistentLayerCore.Repositories;

namespace EntityFramework.Repository.Core.Tests
{
    [TestClass]
    public class RepositoryDeleteTests
    {
        TestExampleDatabaseFactory DatabaseFactory { get; set; } = null!;

        [TestInitialize]
        public void Setup()
        {
            DatabaseFactory = new TestExampleDatabaseFactory();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void DeleteTestMethod()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);

            var actual1 = repository.Count();

            var itemToDelete = repository.Find(1)!;

            repository.Delete(itemToDelete);

            repository.Save();

            var actual2 = repository.Count();

            Assert.AreEqual(7, actual1);
            Assert.AreEqual(6, actual2);

            repository.Dispose();
        }

        [TestCategory("CoreRepository")]
        [TestMethod]
        public void DeleteMultipleTestMethod()
        {
            var repository = new SimpleDataEntityRepository(DatabaseFactory);

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
