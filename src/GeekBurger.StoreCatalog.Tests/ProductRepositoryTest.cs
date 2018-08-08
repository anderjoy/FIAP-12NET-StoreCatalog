using GeekBurger.StoreCatalog.WebAPI.Models;
using GeekBurger.StoreCatalog.WebAPI.Repository;
using System;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;

namespace GeekBurger.StoreCatalog.Tests
{
    public class ProductRepositoryTest
    {
        private readonly StoreContext context;
        private readonly IProductRepository productRepository;

        public ProductRepositoryTest()
        {
            context = new StoreContext();
            productRepository = new ProductRepository(context);
        }

        [Fact]
        public void Include_Two_Products()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            List<Product> products = new List<Product>()
            {
                new Product()
                {
                    Id = id1,
                    Ingredients = "soy,sugar,oil",
                    Items = null
                },
                new Product()
                {
                    Id = id2,
                    Ingredients = "sugar",
                    Items = null
                }
            };

            productRepository.UpsertRangeAsync(products).Wait();

            context.Products.Count(x => x.Id == id1 || x.Id == id2).Should().Be(2, "O resultado dos produtos deveria ser 2");
        }

        [Theory]
        [InlineData("oil")]
        [InlineData("oil,sugar")]
        [InlineData("mustard")]
        public void Include_One_Product_And_ChangeIngredients(string Ingredients)
        {
            var id = Guid.NewGuid();

            Product product = new Product()
            {
                Id = id,
                Ingredients = "",
                Items = null
            };

            productRepository.UpsertAsync(product).Wait();

            product.Ingredients = Ingredients;

            productRepository.UpsertAsync(product).Wait();

            context.Products.Where(x => x.Id == id).Count().Should().Be(1, "O resultado dos produtos deveria ser 1");
            context.Products.FirstOrDefault(x => x.Id == id).Ingredients.Should().Be(Ingredients, $"Os ingredientes deveriam ter sido atualizados para {Ingredients}");
        }

        [Fact]
        public void Include_One_Product_And_Delete()
        {
            var id = Guid.NewGuid();

            Product product = new Product()
            {
                Id = id,
                Ingredients = "",
                Items = null
            };

            productRepository.UpsertAsync(product).Wait();

            context.Products.Where(x => x.Id == id).Count().Should().Be(1, "O resultado dos produtos deveria ser 1");

            productRepository.DeleteAsync(product).Wait();

            context.Products.Where(x => x.Id == id).Count().Should().Be(0, "O resultado dos produtos deveria ser 0");
        }
    }
}
