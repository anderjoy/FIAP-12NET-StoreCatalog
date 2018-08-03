using GeekBurger.Production.Contract.Model;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.Services
{
    public interface IProductionAreaService
    {
        Task<ProductionArea[]> GetProductionAreaAsync();
    }
}
