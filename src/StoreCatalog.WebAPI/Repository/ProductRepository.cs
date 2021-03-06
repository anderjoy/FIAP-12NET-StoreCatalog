﻿using GeekBurger.StoreCatalog.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext _context;

        public ProductRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(Product product)
        {
            var _product = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);

            if (_product != null)
            {
                _context.Remove(_product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpsertAsync(Product product)
        {
            if (_context.Products.AsNoTracking().Any(x => x.Id == product.Id))
            {
                _context.Attach(product);
                _context.Update(product);
                //_context.Entry(product).State = EntityState.Modified;
            }
            else
            {
                await _context.AddAsync(product);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpsertRangeAsync(IList<Product> products)
        {
            foreach (var product in products)
            {
                if (_context.Products.AsNoTracking().Any(x => x.Id == product.Id))
                {
                    _context.Attach(product);
                    _context.Update(product);
                    //_context.Entry(product).State = EntityState.Modified;
                }
                else
                {
                    await _context.AddAsync(product);
                }                
            }

            await _context.SaveChangesAsync();
        }
    }
}
