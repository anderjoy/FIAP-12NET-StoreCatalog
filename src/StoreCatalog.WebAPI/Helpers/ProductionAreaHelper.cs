using GeekBurger.Productions.Contract;
using GeekBurger.StoreCatalog.WebAPI.Models;

namespace GeekBurger.StoreCatalog.WebAPI.Helpers
{
    public static class ProductionAreaHelper
    {
        public static ProductionAreas ToProductionAreas(this ProductionAreaToGet productionArea)
        {
            return new ProductionAreas()
            {
                Id = productionArea.ProductionAreaId,
                Name = "",
                Restrictions = string.Join(',', productionArea.Restrictions),
                Status = productionArea.On
            };
        }
    }
}
