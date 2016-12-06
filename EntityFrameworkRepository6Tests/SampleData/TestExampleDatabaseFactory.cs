using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Effort.Extra;
using PersistentLayer.Entities;
using PersistentLayer;
using PersistentLayer.Contexts;
using EntityFrameworkRepository6.Base;

namespace EntityFrameworkRepository6Tests
{
    public class TestExampleDatabaseFactory : IDatabaseFactory<YourCustomDataContext>
    {
        public Action<string> Logging { get; set; }

        public YourCustomDataContext GetNewDbContext()
        {
            var data = new ObjectData();

            data.Table<SimpleDataEntity>("SimpleDataEntities").Add(new SimpleDataEntity { Id = 1, Name = "Test 1" });
            data.Table<SimpleDataEntity>("SimpleDataEntities").Add(new SimpleDataEntity { Id = 2, Name = "Test 2" });
            data.Table<SimpleDataEntity>("SimpleDataEntities").Add(new SimpleDataEntity { Id = 3, Name = "Test 3" });
            data.Table<SimpleDataEntity>("SimpleDataEntities").Add(new SimpleDataEntity { Id = 4, Name = "Test 4" });
            data.Table<SimpleDataEntity>("SimpleDataEntities").Add(new SimpleDataEntity { Id = 5, Name = "Test 5" });

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
