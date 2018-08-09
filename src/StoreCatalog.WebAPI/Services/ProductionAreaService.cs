using GeekBurger.Productions.Contract;
using GeekBurger.StoreCatalog.WebAPI.Helpers;
using GeekBurger.StoreCatalog.WebAPI.Models;
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
    public class ProductionAreaService : IProductionAreaService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogServiceBus _logServiceBus;

        public ProductionAreaService(IConfiguration configuration, HttpClient httpClient, ILogServiceBus logServiceBus)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logServiceBus = logServiceBus;
        }

        public async Task<IList<ProductionAreas>> GetProductionAreaAsync()
        {
            ProductionAreaToGet[] areas = null;

            try
            {
                await _logServiceBus.SendMessagesAsync($"Recuperando as áreas de produção \"storeName = {_configuration["StoreInfo:StoreName"]}\"...");

                areas = JsonConvert.DeserializeObject<ProductionAreaToGet[]>(await _httpClient.GetStringAsync($"{_configuration["API:ProductionAreas"]}/?storeName={_configuration["StoreInfo:StoreName"]}"));
            }
            catch (Exception E)
            {
                await _logServiceBus.SendMessagesAsync($"Falha ao recuperar os dados das áreas de produção, segue a descrição \"{E.Message}\".");

                //Falha ao acessar o microserviço das áreas de produção
                if (_configuration["API:mocked"] == "true")
                {
                    await _logServiceBus.SendMessagesAsync($"Criando dados fakes para área de produção");

                    areas = new ProductionAreaToGet[]
                    {
                        new ProductionAreaToGet()
                        {
                            ProductionAreaId = Guid.NewGuid(),
                            Restrictions = new List<string>() {"soy", "dairy", "gluten", "sugar"},
                            On = true
                        },
                        new ProductionAreaToGet()
                        {
                            ProductionAreaId = Guid.NewGuid(),
                            Restrictions = new List<string>(),
                            On = true
                        },
                        new ProductionAreaToGet()
                        {
                            ProductionAreaId = Guid.NewGuid(),
                            Restrictions = new List<string>() {"soy"},
                            On = true
                        }
                    };
                }
                else
                {
                    throw;
                }
            }

            return areas.Select(x => x.ToProductionAreas()).ToList();
        }
    }
}
