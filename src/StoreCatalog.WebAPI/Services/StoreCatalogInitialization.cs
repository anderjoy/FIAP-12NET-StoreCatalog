using GeekBurger.Production.Contract.Model;
using GeekBurger.Products.Contract;
using GeekBurger.StoreCatalog.WebAPI.Repository;
using GeekBurger.StoreCatalog.WebAPI.ServiceBus;
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
        private readonly ISendMessageServiceBus _sendMessageServiceBus;
        private readonly IProductRepository _productRepository;

        public StoreCatalogInitialization(IConfiguration configuration, HttpClient httpClient, 
            IProductionAreaService productionAreaService,
            IProductService productService,
            ISendMessageServiceBus sendMessageServiceBus,
            IProductRepository productRepository)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _productionAreaService = productionAreaService;
            _productService = productService;
            _sendMessageServiceBus = sendMessageServiceBus;
            _productRepository = productRepository;

            Task.Run(async () => await InitializeStoreCatalog()).Wait();
        }

        private async Task InitializeStoreCatalog()
        {
            try
            {
                var areas = await _productionAreaService.GetProductionAreaAsync();
                var products = await _productService.GetProductsAsync();

                await _productRepository.UpsertRangeProductAsync(products);

                await _sendMessageServiceBus.SendStoreCatalogReadyAsync();
            }
            catch (Exception)
            {
                throw;
            }            
        }
    }
}
