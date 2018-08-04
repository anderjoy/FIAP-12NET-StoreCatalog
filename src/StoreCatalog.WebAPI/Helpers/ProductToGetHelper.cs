using GeekBurger.Products.Contract;
using StoreCatalog.WebAPI.Models;
using System.Linq;

namespace GeekBurger.StoreCatalog.WebAPI.Helpers
{
    public static class ProductToGetHelper
    {
        public static Product ToProduct(this ProductToGet productToGet)
        {
            return new Product()
            {
                Id = productToGet.ProductId,
                Items = productToGet.Items.Select(i => new Item()
                {
                    Id = i.ItemId,
                    Ingredients = ""
                }).ToList()
            };
        }
    }
}
