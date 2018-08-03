using GeekBurger.Production.Contract.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.Services
{
    public class ProductionAreaService : IProductionAreaService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public ProductionAreaService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<ProductionArea[]> GetProductionAreaAsync()
        {
            ProductionArea[] areas = null;

            try
            {
                areas = JsonConvert.DeserializeObject<ProductionArea[]>(await _httpClient.GetStringAsync(_configuration["API:ProductionAreas"]));
            }
            catch (HttpRequestException)
            {
                //Falha ao acessar o microserviço das áreas de produção

                areas = new ProductionArea[]
                {
                    new ProductionArea()
                    {
                        ProductionId = Guid.NewGuid(),
                        Restrictions = new List<string>() {"soy", "dairy", "gluten", "sugar"},
                        On = true
                    },
                    new ProductionArea()
                    {
                        ProductionId = Guid.NewGuid(),
                        Restrictions = new List<string>(),
                        On = true
                    },
                    new ProductionArea()
                    {
                        ProductionId = Guid.NewGuid(),
                        Restrictions = new List<string>() {"soy"},
                        On = true
                    }
                };
            }

            return areas;
        }
    }
}
