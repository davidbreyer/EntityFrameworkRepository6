## EntityFrameworkRepository6

Entity Framework 6 Repository Pattern

**Getting Started:**

Implement the base repository
```csharp
    public interface ISampleDataEntityRepository : IBaseRepository<YourCustomDataContext, SampleDataEntity>
    {
    }

    public class SampleDataEntityRepository : BaseRepository<YourCustomDataContext, SimpleDataEntity>, ISampleDataEntityRepository
    {
        public SampleDataEntityRepository(IDatabaseFactory<YourCustomDataContext> dbFactory)
        {
            Context = dbFactory.GetNewDbContext();
        }

        //This is where custom query stuff will go.
    }
```
Creating unit tests with an in-memory database with NMemory and Effort.
```csharp
    public class InMemoryDatabaseFactory : IDatabaseFactory<YourCustomDataContext>
    {
        public YourCustomDataContext GetNewDbContext()
        {
            var data = new ObjectData();

            data.Table<SimpleDataEntity>("SimpleDataEntities").Add(new SimpleDataEntity { Id = 1, Name = "Test 1" });
            data.Table<SimpleDataEntity>("SimpleDataEntities").Add(new SimpleDataEntity { Id = 2, Name = "Test 2" });
            data.Table<SimpleDataEntity>("SimpleDataEntities").Add(new SimpleDataEntity { Id = 3, Name = "Test 3" });
            data.Table<SimpleDataEntity>("SimpleDataEntities").Add(new SimpleDataEntity { Id = 4, Name = "Test 4" });
            data.Table<SimpleDataEntity>("SimpleDataEntities").Add(new SimpleDataEntity { Id = 5, Name = "Test 5" });

            var dataLoader = new ObjectDataLoader(data);

            var connection = Effort.DbConnectionFactory.CreateTransient(dataLoader);

            var _context = new YourCustomDataContext(connection);
            
            _context.Database.CreateIfNotExists();
            _context.Database.Initialize(true);

            return _context;
        }
    }
```
[Getting Started with the Audit Context]
