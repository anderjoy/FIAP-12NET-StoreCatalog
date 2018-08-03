using GeekBurger.Production.Contract.Model;
using GeekBurger.Products.Contract;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.Services
{
    public class StoreCatalogInitialization : IStoreCatalogInitialization
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IProductionAreaService _productionAreaService;
        private readonly IProductService _productService;

        public StoreCatalogInitialization(IConfiguration configuration, HttpClient httpClient, 
            IProductionAreaService productionAreaService,
            IProductService productService)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _productionAreaService = productionAreaService;
            _productService = productService;

            Task.Run(async () => await InitializeStoreCatalog());
        }

        private async Task InitializeStoreCatalog()
        {
            try
            {
                var areas = await _productionAreaService.GetProductionAreaAsync();
                var products = await _productService.GetProductsAsync();
            }
            catch (Exception)
            {
            }            
        }
    }
}
