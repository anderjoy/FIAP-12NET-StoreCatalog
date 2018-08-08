using FluentAssertions;
using GeekBurger.StoreCatalog.WebAPI.Models;
using GeekBurger.StoreCatalog.WebAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GeekBurger.StoreCatalog.Tests
{
    public class ProductionAreaRepositoryTest
    {
        private readonly StoreContext context;
        private readonly IProductionAreaRepository productionAreaRepository;

        public ProductionAreaRepositoryTest()
        {
            context = new StoreContext();
            productionAreaRepository = new ProductionAreaRepository(context);
        }

        [Fact]
        public void Include_Two_ProductionAreas()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            List<ProductionAreas> products = new List<ProductionAreas>()
            {
                new ProductionAreas()
                {
                    Id = id1,
                    Name = "Steira 1",
                    Restrictions = "sugar,oil",
                    Status = true
                },
                new ProductionAreas()
                {
                    Id = id2,
                    Name = "Steira 1",
                    Restrictions = "sugar,oil",
                    Status = true
                }
            };

            productionAreaRepository.UpsertRangeAsync(products).Wait();

            context.ProductionAreas.Count(x => x.Id == id1 || x.Id == id2).Should().Be(2, "O resultado das áreas de produção deveria ser 2");
        }

        [Theory]
        [InlineData("Steira XXX")]
        [InlineData("Steira YYY")]
        [InlineData("Steira ZZZ")]
        public void Include_One_ProductionArea_And_ChangeName(string Name)
        {
            var id = Guid.NewGuid();

            ProductionAreas productionArea = new ProductionAreas()
            {
                Id = id,
                Name = "Steira 1",
                Restrictions = "sugar,oil",
                Status = true
            };

            productionAreaRepository.UpsertAsync(productionArea).Wait();

            productionArea.Name = Name;

            productionAreaRepository.UpsertAsync(productionArea).Wait();

            context.ProductionAreas.Count(x => x.Id == id).Should().Be(1, "O resultado dos produtos deveria ser 1");
            context.ProductionAreas.FirstOrDefault(x => x.Id == id).Name.Should().Be(Name, $"O nome da área de produção deveria ter sido alterado para {Name}");
        }
    }
}
