using Microsoft.EntityFrameworkCore;

namespace StoreCatalog.WebAPI.Models
{
    public class StoreContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductionAreas> ProductionAreas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("StoreCatalog");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>().HasKey(x => x.Id);

            modelBuilder.Entity<Product>().HasKey(x => x.Id);

            modelBuilder.Entity<ProductionAreas>().HasKey(x => x.Id);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Items)
                .WithOne(i => i.Product);
        }
    }
}