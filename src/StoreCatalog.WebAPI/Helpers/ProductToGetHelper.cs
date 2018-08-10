using GeekBurger.Products.Contract;
using GeekBurger.StoreCatalog.WebAPI.Models;
using GeekBurguer.Ingredients.Contracts;
using System.Collections.Generic;
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
                Ingredients = "",
                Items = productToGet.Items.Select(i => new Item()
                {
                    Id = i.ItemId
                }).ToList()
            };
        }

        public static Product ToProduct(this IngredientsRestrictionsResponse ingredientsRestrictions)
        {
            return new Product()
            {
                Id = ingredientsRestrictions.ProductId,
                Ingredients = string.Join(',', ingredientsRestrictions.Ingredients).ToLower(),
                Items = new List<Item>()
            };
        }
    }
}
