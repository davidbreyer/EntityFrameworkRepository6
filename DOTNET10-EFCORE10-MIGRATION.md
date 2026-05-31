# .NET 10 / EF Core 10 Migration Assessment

Branch: `upgrade/dotnet10-efcore10`

## Current State

- Solution uses Visual Studio 2015-era non-SDK-style `.csproj` files.
- Projects target `.NET Framework 4.5` and `.NET Framework 4.5.1`.
- NuGet uses `packages.config`.
- Runtime dependency is Entity Framework 6.4.4.
- Main repository package is small and mostly CRUD/update/query wrappers over `DbContext`.
- Audit support is a separate EF6-specific layer and can be removed for the .NET 10 version.
- Tests use MSTest plus EF6-only in-memory tooling: `Effort.EF6`, `Effort.Extra.EF6`, and `NMemory`.

## Recommended Target

- `net10.0`
- SDK-style projects.
- `PackageReference`.
- `Microsoft.EntityFrameworkCore` 10.x.
- Provider package selected by consuming app or test/sample project, usually:
  - `Microsoft.EntityFrameworkCore.SqlServer`
  - `Microsoft.EntityFrameworkCore.Sqlite` for in-memory integration tests
  - optionally `Microsoft.EntityFrameworkCore.InMemory` for lightweight behavior tests

## Main Breaking Changes

### EF namespace and base APIs

Current code uses `System.Data.Entity`. EF Core uses `Microsoft.EntityFrameworkCore`.

The main repository code maps reasonably well:

- `DbContext`
- `DbSet<T>`
- `AsNoTracking()`
- `Find` / `FindAsync`
- `Entry(entity).CurrentValues.SetValues(...)`
- `Entry(entity).Property(...).IsModified`
- `Reload` / `ReloadAsync`
- `SaveChanges` / `SaveChangesAsync`

Expected code changes are mostly namespace changes, nullable compatibility fixes, and async signature fixes.

### DbContext construction

EF6 supports constructors like:

```csharp
public YourContext(string connectionString) : base(connectionString) { }
```

EF Core prefers:

```csharp
public YourContext(DbContextOptions<YourContext> options) : base(options) { }
```

This affects:

- `BaseContext<TContext>`
- `DatabaseFactory<C>`
- sample contexts
- tests
- consuming apps that currently pass raw connection strings into context constructors

To reduce downstream pain, the new library can keep an `IDatabaseFactory<C>` abstraction, but it should create contexts from `DbContextOptions<C>` or a caller-provided factory delegate instead of using `Activator.CreateInstance` with a string.

### Removed EF6 database initializer model

EF6 APIs such as these do not exist in EF Core:

- `Database.SetInitializer<TContext>(...)`
- `IDatabaseInitializer<TContext>`
- `Database.Initialize(...)`
- EF6 `DbMigrationsConfiguration`
- EF6 `AddOrUpdate`

Replacement options:

- Use EF Core migrations.
- Use `context.Database.Migrate()` where appropriate in apps or samples.
- Seed test data directly in tests.
- Use `HasData(...)` only for stable static seed data.

### Logging

EF6 `context.Database.Log = action` does not exist in EF Core.

Replacement options:

- Configure logging through `DbContextOptionsBuilder.LogTo(...)`.
- Let consuming apps wire logging with `ILoggerFactory`.

`IDatabaseFactory<C>.Logging` should probably be removed or replaced with an options-builder callback.

### Audit library removal

The audit projects are tightly EF6-specific:

- `AuditDbContextLocal`
- `AuditDbContextTests`
- `EntityFrameworkAuditableRepository6`
- `EntityFrameworkAuditableRepository6Tests`
- `PersistentLayerAuditable`

They use EF6 APIs and assumptions that should not be carried forward:

- `System.Data.Entity.Infrastructure.DbEntityEntry`
- dynamic proxy namespace checks
- `DbSet.Create()`
- `Database.SqlQuery<T>()`
- App.config custom configuration sections
- EF6 change tracking behavior

Recommendation: remove these projects from the .NET 10 solution/package set and publish the new version as the non-auditable repository only. If auditing is needed later, rebuild it separately around EF Core interceptors or `SaveChanges` overrides.

### Tests

The existing test setup cannot be directly upgraded because Effort/NMemory are EF6-specific.

Recommended replacement:

- Convert tests to SDK-style MSTest or xUnit.
- Use SQLite in-memory for relational behavior.
- Keep the same repository behavior tests:
  - add
  - update
  - delta update
  - delete
  - find by key
  - composite key
  - read-only query
  - reload
  - save
  - count

SQLite in-memory is the better default because it exercises relational behavior more realistically than `Microsoft.EntityFrameworkCore.InMemory`.

## Suggested Migration Phases

### Phase 1: Produce a minimal EF Core package

Scope:

- Keep only:
  - `EntityFramework.SharedRepository`
  - `EntityFrameworkRepository6`
  - non-auditable sample/test projects
- Convert the repository project to SDK-style `net10.0`.
- Replace EF6 references with EF Core references.
- Update repository code to compile.
- Replace string-based context factory with an EF Core-friendly factory.

Expected effort: 1-2 focused days.

Risk: medium, mostly around preserving the public API enough for existing apps.

### Phase 2: Rebuild tests

Scope:

- Convert tests to SDK-style.
- Replace Effort/NMemory with SQLite in-memory.
- Port existing behavior tests.
- Add nullable and async coverage.

Expected effort: 1-2 focused days.

Risk: medium, because tests may expose EF6 vs EF Core behavior differences.

### Phase 3: Package and compatibility cleanup

Scope:

- Decide package name and versioning.
- Update nuspec usage or replace with SDK-generated NuGet package metadata.
- Update README examples.
- Add GitHub Actions CI for `dotnet build`, `dotnet test`, and `dotnet pack`.

Expected effort: half day to 1 day.

Risk: low.

### Phase 4: App-by-app adoption

Scope per consuming app:

- Move app data contexts to EF Core options-based constructors.
- Install provider package in the app.
- Update registrations in DI.
- Recreate or port migrations as needed.
- Validate queries that depended on EF6 translation behavior.

Expected effort: varies by app, but usually more than the library itself.

Risk: medium to high depending on each app's EF usage.

## Rough Overall Estimate

For the library itself, without the audit layer:

- Small proof-of-concept compile: 1 day.
- Usable package with tests: 3-5 days.
- Polished NuGet-ready release with CI/docs: about 1 week.

For numerous consuming apps, the real cost will be in context construction, migrations, provider configuration, and EF6 query behavior differences in each app.

## Local Environment Note

This machine currently has .NET SDK `8.0.421` installed. A real `net10.0` build will require installing the .NET 10 SDK.
