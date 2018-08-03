using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GeekBurguer.StoreCatalog.Contract
{
    public class Item
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("ingredients")]
        public IEnumerable<string> Ingredients { get; set; }
    }
}