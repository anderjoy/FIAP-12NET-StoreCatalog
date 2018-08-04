using GeekBurger.Products.Contract;
using StoreCatalog.WebAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.Services
{
    public interface IProductService
    {
        Task<IList<Product>> GetProductsAsync();
    }
}
