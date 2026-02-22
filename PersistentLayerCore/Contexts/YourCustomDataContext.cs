using Microsoft.EntityFrameworkCore;
using PersistentLayerCore.Entities;

namespace PersistentLayerCore.Contexts
{
    public class YourCustomDataContext : DbContext
    {
        public DbSet<SimpleDataEntity> SimpleDataEntities { get; set; } = null!;
        public DbSet<SimpleCompositeKeyEntity> SimpleCompositeKeyEntities { get; set; } = null!;

        public YourCustomDataContext(DbContextOptions<YourCustomDataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SimpleCompositeKeyEntity>()
                .HasKey(e => new { e.Id, e.Name });
        }
    }
}
