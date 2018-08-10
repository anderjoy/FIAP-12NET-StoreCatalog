using GeekBurger.StoreCatalog.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.Services
{
    public interface IProductService
    {
        Task<IList<Product>> GetProductsAsync();

        Task<IList<Product>> GetProductsByRestrictionsAsync(string storeName, string[] Restrictions);
    }
}
