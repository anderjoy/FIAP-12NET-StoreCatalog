using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GeekBurger.Production.Contract.Model;
using GeekBurger.Products.Contract;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StoreCatalog.WebAPI.Models;

namespace StoreCatalog.WebAPI.Controllers
{
    [Route("api")]
    public class StoreCatalogController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly StoreContext _storeContext;

        public StoreCatalogController(HttpClient httpClient, IConfiguration configuration, StoreContext storeContext)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _storeContext = storeContext ?? throw new ArgumentNullException(nameof(storeContext));
        }

        [Route("store/")]
        public async Task<IActionResult> Index()
        {
            //verificar se tem produtos, areas blablabla
            //Verifica as areas
            var areas = JsonConvert.DeserializeObject<ProductionArea[]>(await _httpClient.GetStringAsync(_configuration["ProductionAreas"]));
            var products = JsonConvert.DeserializeObject<ProductToGet[]>(await _httpClient.GetStringAsync(_configuration["Products"]));

            foreach (var product in products)
            {
                if (!_storeContext.Products.Any(c => c.Name == product.Name))
                {
                    var productUpsert = new ProductToUpsert()
                    {
                        Image = product.Image,
                        Name = product.Name,
                        Price = product.Price,
                        
                    };
                    _storeContext.Products.Add(new ProductToUpsert()
                    {
                      

                    });
                }
                    
            }

            return StatusCode(503);
        }

        [HttpGet]
        public async Task<IActionResult> Get(string[] restrictions)
        {
            
            


            //Grava os produtos no banco
            //await _storeContext.Products.AddRangeAsync(products);
            await _storeContext.SaveChangesAsync();

            return Ok(areas);
        }
    }
}