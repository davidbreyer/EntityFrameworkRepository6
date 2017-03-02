using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Effort.Extra;
using PersistentLayer.Auditable.Entities;
using PersistentLayer.Auditable;
using PersistentLayer.Auditable.Contexts;
using EntityFramework.Repository6;

namespace EntityFramework.Auditable.Repository6.Tests
{
    public class TestExampleDatabaseFactory : IDatabaseFactory<YourCustomDataContext>
    {
        public Action<string> Logging { get; set; }

        public YourCustomDataContext GetNewDbContext()
        {
            var data = new ObjectData();

            data.Table<SimpleDataEntity>("SimpleDataEntities").Add(new SimpleDataEntity { Id = 1, Name = "Test 1", UpdateUser = "Daisy", Updated = new DateTimeOffset(2017, 1, 22, 5, 32, 23, 0, new TimeSpan(3, 0, 0)) });
            data.Table<SimpleDataEntity>("SimpleDataEntities").Add(new SimpleDataEntity { Id = 2, Name = "Test 2", UpdateUser = "Phil", Updated = new DateTimeOffset(2017, 1, 22, 5, 32, 23, 0, new TimeSpan(3, 0, 0)) });
            data.Table<SimpleDataEntity>("SimpleDataEntities").Add(new SimpleDataEntity { Id = 3, Name = "Test 3", UpdateUser = "Melinda", Updated = new DateTimeOffset(2017, 1, 22, 5, 32, 23, 0, new TimeSpan(3, 0, 0)) });
            data.Table<SimpleDataEntity>("SimpleDataEntities").Add(new SimpleDataEntity { Id = 4, Name = "Test 4", UpdateUser = "Leo", Updated = new DateTimeOffset(2017, 1, 22, 5, 32, 23, 0, new TimeSpan(3, 0, 0)) });
            data.Table<SimpleDataEntity>("SimpleDataEntities").Add(new SimpleDataEntity { Id = 5, Name = "Test 5", UpdateUser = "Jemma", Updated = new DateTimeOffset(2017, 1, 22, 5, 32, 23, 0, new TimeSpan(3, 0, 0)) });

            data.Table<SimpleCompositeKeyEntity>("SimpleCompositeKeyEntities").Add(new SimpleCompositeKeyEntity { Id = 1, Name = "Another Key" });

            var dataLoader = new ObjectDataLoader(data);

            var connection = Effort.DbConnectionFactory.CreateTransient(dataLoader);

            var _context = new YourCustomDataContext(connection);
            
            _context.Database.CreateIfNotExists();
            _context.Database.Initialize(true);

            if (Logging != null) { _context.Database.Log = Logging; }

            return _context;
        }
    }
}
