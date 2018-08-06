using GeekBurger.StoreCatalog.WebAPI.Models;
using GeekBurger.StoreCatalog.WebAPI.ServiceBus;
using GeekBurger.StoreCatalog.WebAPI.Services;
using GeekBurguer.StoreCatalog.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
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
        private readonly IProductService _productService;
        private readonly StoreContext _context;

        public StoreCatalogController(HttpClient httpClient, IConfiguration configuration, StoreContext storeContext,
            ISendMessageServiceBus sendMessageServiceBus, IProductService productService)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _context = storeContext ?? throw new ArgumentNullException(nameof(storeContext));

            _sendMessageServiceBus = sendMessageServiceBus;
            _productService = productService;
        }

        [Route("store/")]
        [HttpGet]
        public IActionResult Index()
        {
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

                var filteredProducts = await _productService.GetProductsByRestrictionsAsync(user.Restrictions);

                var allowedProducts = filteredProducts
                    .Where(product => product.Ingredients.All(ingredient => allowedAreas.Any(area => !area.Restrictions.Contains(ingredient))));

                if (allowedProducts.Count() <= 2)
                {
                    //Manda pro service bus a mensagem
                    await _sendMessageServiceBus.SendUserWithLessOffer(new
                    {
                        UserId = user.Id,
                        user.Restrictions
                    });
                }

                return Ok(allowedProducts);
            }
            catch (HttpRequestException)
            {

                throw;
            }

        }
    }
}