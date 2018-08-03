using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.Services
{
    public class StoreCatalogInitialization : IStoreCatalogInitialization
    {
        private readonly IConfiguration _configuration;

        public StoreCatalogInitialization(IConfiguration configuration)
        {
            _configuration = configuration;

            //
        }
    }
}
