﻿using GeekBurger.StoreCatalog.WebAPI.Models;
using GeekBurger.StoreCatalog.WebAPI.ServiceBus;
using GeekBurger.StoreCatalog.WebAPI.Services;
using GeekBurguer.StoreCatalog.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IProductService _productService;
        private readonly ILogServiceBus _logServiceBus;
        private readonly StoreContext _context;

        public StoreCatalogController(HttpClient httpClient, IConfiguration configuration, StoreContext storeContext,
            ISendMessageServiceBus sendMessageServiceBus, IProductService productService, ILogServiceBus logServiceBus)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _context = storeContext ?? throw new ArgumentNullException(nameof(storeContext));

            _sendMessageServiceBus = sendMessageServiceBus;
            _productService = productService;
            _logServiceBus = logServiceBus;
        }

        [Route("store/")]
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Index()
        {
            await _logServiceBus.SendMessagesAsync("Required method: /api/store/");

            return Ok();
        }

        [HttpGet]
        [Route("products")]
        [ProducesResponseType(200, Type = typeof(List<GeekBurger.StoreCatalog.WebAPI.Models.Product>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get(User user)
        {
            await _logServiceBus.SendMessagesAsync("Required method: /api/products/");

            try
            {                
                var areas = await _context.ProductionAreas.ToListAsync();                
                var allowedAreas = areas.Where(area => user.Restrictions.All(restriction => area.Restrictions.Contains(restriction.ToLower())))
                    .ToList();

                var filteredProducts = await _productService.GetProductsByRestrictionsAsync(user.StoreNome, user.Restrictions);

                var allowedProducts = filteredProducts
                    .Where(product => product.ListIngredients.All(ingredient => allowedAreas.Any(area => !area.ListRestrictions.Contains(ingredient))))
                    .ToList();

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
            catch (Exception E)
            {
                await _logServiceBus.SendMessagesAsync($"Falha ao processar a requisão \"/api/products\", segue a descrição \"{E.Message}\".");
                throw;
            }

        }
    }
}