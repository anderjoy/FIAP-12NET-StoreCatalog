using GeekBurger.StoreCatalog.WebAPI.Repository;
using GeekBurger.StoreCatalog.WebAPI.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
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
        private readonly IProductionAreaRepository _productionAreaRepository;
        private readonly ILogServiceBus _logServiceBus;

        public StoreCatalogInitialization(IConfiguration configuration, HttpClient httpClient, 
            IProductionAreaService productionAreaService,
            IProductService productService,
            ISendMessageServiceBus sendMessageServiceBus,
            IProductRepository productRepository,
            IProductionAreaRepository productionAreaRepository,
            ILogServiceBus logServiceBus)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _productionAreaService = productionAreaService;
            _productService = productService;
            _sendMessageServiceBus = sendMessageServiceBus;
            _productRepository = productRepository;
            _productionAreaRepository = productionAreaRepository;
            _logServiceBus = logServiceBus;
        }

        public async Task InitializeStoreCatalog()
        {
            try
            {
                await _logServiceBus.SendMessagesAsync("Inicializando...");
                var areas = await _productionAreaService.GetProductionAreaAsync();
                var products = await _productService.GetProductsAsync();

                await _logServiceBus.SendMessagesAsync("Atualizando base de dados...");
                await _productRepository.UpsertRangeAsync(products);
                await _productionAreaRepository.UpsertRangeAsync(areas);

                await _sendMessageServiceBus.SendStoreCatalogReadyAsync();

                await _logServiceBus.SendMessagesAsync("Inicializado com sucesso.");
            }
            catch (Exception)
            {
                throw;
            }            
        }
    }
}
