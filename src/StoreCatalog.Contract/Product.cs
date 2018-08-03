using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GeekBurguer.StoreCatalog.Contract
{
    public class Product
    {
        public Guid Id { get; set; }

        public IEnumerable<Item> Items { get; set; }

        public static IEnumerable<Product> GetProducts(string json)
        {
            return JsonConvert.DeserializeObject<IEnumerable<Product>>(json);
        }
    }
}
