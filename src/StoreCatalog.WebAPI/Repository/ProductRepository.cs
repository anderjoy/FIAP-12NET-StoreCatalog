using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreCatalog.WebAPI.Models;

namespace GeekBurger.StoreCatalog.WebAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext _context;

        public ProductRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task UpsertProductAsync(Product product)
        {
            if (_context.Products.Any(x => x.Id == product.Id))
            {
                _context.Attach(product);
                _context.Update(product);
            }
            else
            {
                await _context.AddAsync(product);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpsertRangeProductAsync(IList<Product> products)
        {
            foreach (var product in products)
            {
                await UpsertProductAsync(product);
            }
        }
    }
}
