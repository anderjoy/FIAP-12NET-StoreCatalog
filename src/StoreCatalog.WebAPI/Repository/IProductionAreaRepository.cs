using GeekBurger.StoreCatalog.WebAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.Repository
{
    public interface IProductionAreaRepository
    {
        Task UpsertAsync(ProductionAreas productionAreas);

        Task UpsertRangeAsync(IList<ProductionAreas> productionAreas);
    }
}
