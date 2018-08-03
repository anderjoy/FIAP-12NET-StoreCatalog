using GeekBurger.Products.Contract;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.Services
{
    public interface IProductService
    {
        Task<ProductToGet[]> GetProductsAsync();
    }
}
