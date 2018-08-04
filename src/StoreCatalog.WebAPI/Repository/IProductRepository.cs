using StoreCatalog.WebAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.Repository
{
    public interface IProductRepository
    {
        Task UpsertProductAsync(Product product);

        Task UpsertRangeProductAsync(IList<Product> products);
    }
}
