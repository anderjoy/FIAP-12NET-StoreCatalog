﻿using GeekBurger.Products.Contract;
using GeekBurger.StoreCatalog.WebAPI.Helpers;
using GeekBurger.StoreCatalog.WebAPI.Models;
using GeekBurger.StoreCatalog.WebAPI.ServiceBus;
using GeekBurguer.Ingredients.Contracts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
        private readonly ILogServiceBus _logServiceBus;

        public ProductService(IConfiguration configuration, HttpClient httpClient, ILogServiceBus logServiceBus)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logServiceBus = logServiceBus;
        }

        public async Task<IList<Product>> GetProductsAsync()
        {
            ProductToGet[] productsToGet = null;

            try
            {
                await _logServiceBus.SendMessagesAsync($"Recuperando produtos \"storeName = {_configuration["StoreInfo:StoreName"]}\"...");

                productsToGet = JsonConvert.DeserializeObject<ProductToGet[]>(await _httpClient.GetStringAsync($"{_configuration["API:Products"]}/?storeName={_configuration["StoreInfo:StoreName"]}"));
            }
            catch (Exception E)
            {
                await _logServiceBus.SendMessagesAsync($"Falha ao recuperar os dados dos produtos, segue a descrição \"{E.Message}\".");

                //Falha ao acessar o microserviço de produtos

                if (_configuration["API:mocked"] == "true")
                {
                    await _logServiceBus.SendMessagesAsync($"Criando dados fakes para produtos");

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
                else
                {
                    throw;
                }
            }

            return productsToGet.Select(x => x.ToProduct()).ToList();
        }

        public async Task<IList<Product>> GetProductsByRestrictionsAsync(string[] Restrictions)
        {
            IList<IngredientsRestrictionsResponse> ingredientsRestrictions = null;

            try
            {
                IngredientsRestrictionsRequest ingredientsRestrictionsRequest = new IngredientsRestrictionsRequest()
                {
                    Restrictions = Restrictions.ToList(),
                    StoreId = Guid.Parse(_configuration["StoreInfo:StoreId"])
                };

                var content = new StringContent(JsonConvert.SerializeObject(ingredientsRestrictionsRequest));
                var result = await _httpClient.PostAsync($"{_configuration["API:Products"]}/byrestrictions", content);

                if (result.IsSuccessStatusCode)
                {
                    ingredientsRestrictions = JsonConvert.DeserializeObject<IList<IngredientsRestrictionsResponse>>(await result.Content.ReadAsStringAsync());
                }
                else
                {
                    throw new Exception(result.StatusCode.ToString());
                }
            }
            catch (Exception)
            {
                if (_configuration["API:mocked"] == "true")
                {
                    var listProducts = new List<IngredientsRestrictionsResponse>()
                    {
                        new IngredientsRestrictionsResponse()
                        {
                            Ingredients = new List<string>() { "soy", "sugar" },
                            ProductId = Guid.NewGuid()
                        },
                        new IngredientsRestrictionsResponse()
                        {
                            Ingredients = new List<string>() { "mustard", "onion" },
                            ProductId = Guid.NewGuid()
                        },
                        new IngredientsRestrictionsResponse()
                        {
                            Ingredients = new List<string>() { "lettuce", "olive oil" },
                            ProductId = Guid.NewGuid()
                        }
                    };

                    ingredientsRestrictions = listProducts.Where(produto => produto.Ingredients.All(ingrediente => !Restrictions.Contains(ingrediente)))
                        .ToList();
                }
                else
                {
                    throw;
                }                    
            }

            return ingredientsRestrictions.Select(x => x.ToProduct()).ToList();
        }
    }
}
