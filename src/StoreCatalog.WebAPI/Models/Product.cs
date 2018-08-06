using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekBurger.StoreCatalog.WebAPI.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Ingredients { get; set; }

        public virtual IEnumerable<Item> Items { get; set; }

        [NotMapped]
        public IList<string> ListIngredients => Ingredients?.Split(',');
    }
}
