using GeekBurger.Products.Contract;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StoreCatalog.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public ProductService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<IList<Product>> GetProductsAsync()
        {
            ProductToGet[] productsToGet = null;

            try
            {
                productsToGet = JsonConvert.DeserializeObject<ProductToGet[]>(await _httpClient.GetStringAsync(_configuration["API:Products"]));
            }
            catch (HttpRequestException)
            {
                //Falha ao acessar o microserviço de produtos

                productsToGet = new ProductToGet[]
                {
                    new ProductToGet()
                    {
                        ProductId = Guid.NewGuid(),
                        Items = new List<ItemToGet>()
                        {
                            new ItemToGet() { ItemId = Guid.NewGuid(), Name = "beef" },
                            new ItemToGet() { ItemId = Guid.NewGuid(), Name = "mustard" },
                            new ItemToGet() { ItemId = Guid.NewGuid(), Name = "bread" }
                        }
                    },
                    new ProductToGet()
                    {
                        ProductId = Guid.NewGuid(),
                        Items = new List<ItemToGet>()
                        {
                            new ItemToGet() { ItemId = Guid.NewGuid(), Name = "ketchup" },
                            new ItemToGet() { ItemId = Guid.NewGuid(), Name = "bread" }
                        }
                    },
                    new ProductToGet()
                    {
                        ProductId = Guid.NewGuid(),
                        Items = new List<ItemToGet>()
                        {
                            new ItemToGet() { ItemId = Guid.NewGuid(), Name = "salmon" },
                            new ItemToGet() { ItemId = Guid.NewGuid(), Name = "onion wings" },
                            new ItemToGet() { ItemId = Guid.NewGuid(), Name = "whole bread" }
                        }
                    }
                };
            }

            return productsToGet.Select(x => new Product()
            {
                Id = x.ProductId,
                Items = x.Items.Select(i => new Item()
                {
                    Id = i.ItemId,
                    Ingredients = ""
                })
            }).ToList();

        }
    }
}
