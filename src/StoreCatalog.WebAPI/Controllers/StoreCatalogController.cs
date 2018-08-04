using GeekBurger.Productions.Contract;
using GeekBurger.StoreCatalog.WebAPI.ServiceBus;
using GeekBurguer.Ingredients.Contracts;
using GeekBurguer.StoreCatalog.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StoreCatalog.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StoreCatalog.WebAPI.Controllers
{
    [Route("api")]
    public class StoreCatalogController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ISendMessageServiceBus _sendMessageServiceBus;
        private readonly StoreContext _context;

        public StoreCatalogController(HttpClient httpClient, IConfiguration configuration, StoreContext storeContext,
            ISendMessageServiceBus sendMessageServiceBus)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _context = _context ?? throw new ArgumentNullException(nameof(storeContext));
            _sendMessageServiceBus = sendMessageServiceBus;
        }

        [Route("store/")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //verificar se tem produtos, areas blablabla
            //Verifica as areas
            //var areas = JsonConvert.DeserializeObject<ProductionArea[]>(await _httpClient.GetStringAsync(_configuration["ProductionAreas"]));
            //var products = JsonConvert.DeserializeObject<ProductToGet[]>(await _httpClient.GetStringAsync(_configuration["Products"]));

            //foreach (var product in products)
            //{
            //    if (!_storeContext.Products.Any(c => c.Name == product.Name))
            //    {
            //        var productUpsert = new ProductToUpsert()
            //        {
            //            Image = product.Image,
            //            Name = product.Name,
            //            Price = product.Price,

            //        };
            //        _storeContext.Products.Add(new ProductToUpsert()
            //        {


            //        });
            //    }

            //}

            return Ok();
        }

        [HttpGet]
        [Route("products")]
        public async Task<IActionResult> Get(User user)
        {
            try
            {                
                var urlIngredients = _configuration["Ingredients"];
                var areas = await _context.ProductionAreas.ToListAsync();                
                var allowedAreas = areas.Where(area => user.Restrictions.All(restriction => area.Restrictions.Contains(restriction)));
                //var filteredProducts = JsonConvert.DeserializeObject<IEnumerable<MergeProductsAndIngredients>>(await _httpClient.GetStringAsync(urlIngredients));

                //Monta os dados da requisicao
                var dados = new
                {
                    user.Restrictions,
                    StoreId = _configuration["StoreInfo:StoreId"]
                };

                var content = new StringContent(JsonConvert.SerializeObject(dados));
                var result = await _httpClient.PostAsync($"{_configuration["API:Products"]}/byrestrictions", content);
                var filteredProducts = JsonConvert.DeserializeObject<IEnumerable<IngredientsRestrictionsResponse>>(await result.Content.ReadAsStringAsync());

                var allowedProducts = filteredProducts
                    .Where(product => product.Ingredients.All(ingredient => allowedAreas.Any(area => !area.Restrictions.Contains(ingredient))));

                //Manda pro service bus a mensagem
                await _sendMessageServiceBus.SendUserWithLessOffer(new
                {
                    UserId = user.Id,
                    user.Restrictions
                });
                return Ok(allowedProducts);
            }
            catch (HttpRequestException req)
            {

                throw;
            }

        }
    }
}