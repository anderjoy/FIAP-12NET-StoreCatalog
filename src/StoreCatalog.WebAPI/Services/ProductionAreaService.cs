using GeekBurger.Productions.Contract;
using GeekBurger.StoreCatalog.WebAPI.Helpers;
using GeekBurger.StoreCatalog.WebAPI.Models;
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

        public ProductionAreaService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<IList<ProductionAreas>> GetProductionAreaAsync()
        {
            ProductionAreaToGet[] areas = null;

            try
            {
                areas = JsonConvert.DeserializeObject<ProductionAreaToGet[]>(await _httpClient.GetStringAsync(_configuration["API:ProductionAreas"]));
            }
            catch (Exception)
            {
                //Falha ao acessar o microserviço das áreas de produção
                if (_configuration["API:mocked"] == "true")
                {
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
