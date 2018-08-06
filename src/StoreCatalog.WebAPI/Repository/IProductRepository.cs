using GeekBurger.StoreCatalog.WebAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.Repository
{
    public interface IProductRepository
    {
        Task UpsertAsync(Product product);

        Task UpsertRangeAsync(IList<Product> products);

        Task DeleteAsync(Product product);
    }
}
