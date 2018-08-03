using System;
using System.Collections.Generic;

namespace StoreCatalog.WebAPI.Models
{
    public class Item
    {
        public Guid Id { get; set; }

        public IEnumerable<string> Ingredients { get; set; }

        public virtual Product Product { get; set; }
    }
}