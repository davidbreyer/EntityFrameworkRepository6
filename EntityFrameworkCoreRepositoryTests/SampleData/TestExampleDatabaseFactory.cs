using EntityFramework.Repository.Core;
using Microsoft.EntityFrameworkCore;
using PersistentLayerCore.Contexts;
using PersistentLayerCore.Entities;
using System;

namespace EntityFramework.Repository.Core.Tests
{
    public class TestExampleDatabaseFactory : IDatabaseFactory<YourCustomDataContext>
    {
        public YourCustomDataContext GetNewDbContext()
        {
            var options = new DbContextOptionsBuilder<YourCustomDataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new YourCustomDataContext(options);

            context.SimpleDataEntities.Add(new SimpleDataEntity { Id = 1, Name = "Test 1" });
            context.SimpleDataEntities.Add(new SimpleDataEntity { Id = 2, Name = "Test 2" });
            context.SimpleDataEntities.Add(new SimpleDataEntity { Id = 3, Name = "Test 3" });
            context.SimpleDataEntities.Add(new SimpleDataEntity { Id = 4, Name = "Test 4" });
            context.SimpleDataEntities.Add(new SimpleDataEntity { Id = 5, Name = "Test 5" });
            context.SimpleDataEntities.Add(new SimpleDataEntity { Id = 6, Name = "Test 6" });
            context.SimpleDataEntities.Add(new SimpleDataEntity { Id = 7, Name = "Test 7" });

            context.SimpleCompositeKeyEntities.Add(new SimpleCompositeKeyEntity { Id = 1, Name = "Another Key" });

            context.SaveChanges();

            return context;
        }
    }
}
