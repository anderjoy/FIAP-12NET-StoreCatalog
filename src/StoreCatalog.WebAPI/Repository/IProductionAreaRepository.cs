using StoreCatalog.WebAPI.Models;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.Repository
{
    public interface IProductionAreaRepository
    {
        Task UpsertAsync(ProductionAreas productionAreas);
    }
}
