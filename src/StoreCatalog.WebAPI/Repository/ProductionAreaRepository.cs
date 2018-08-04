﻿using Microsoft.EntityFrameworkCore;
using StoreCatalog.WebAPI.Models;
using System.Linq;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.Repository
{
    public class ProductionAreaRepository : IProductionAreaRepository
    {
        private readonly StoreContext _context;

        public ProductionAreaRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task UpsertAsync(ProductionAreas productionAreas)
        {
            if (_context.ProductionAreas.AsNoTracking().Any(x => x.Id == productionAreas.Id))
            {
                _context.Attach(productionAreas);
                _context.Update(productionAreas);
            }
            else
            {
                await _context.AddAsync(productionAreas);
            }

            await _context.SaveChangesAsync();
        }
    }
}