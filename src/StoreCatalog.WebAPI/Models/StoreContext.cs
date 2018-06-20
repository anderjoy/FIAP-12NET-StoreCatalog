using GeekBurger.Products.Contract;
using GeekBurger.StoreCatalog.Contract.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCatalog.WebAPI.Models
{
    public class StoreContext : DbContext
    {
        public DbSet<StoreCatalogReady> StoreCatalogs { get; set; }
        public DbSet<ProductToGet> Products { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("StoreCatalog");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreCatalogReady>()
                .HasKey(c => c.Id);
        }
    }
}