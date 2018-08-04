using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreCatalog.WebAPI.Models
{
    public class Item
    {
        public Guid Id { get; set; }

        public string Ingredients { get; set; }

        public virtual Product Product { get; set; }

        [NotMapped]
        public IList<string> ListIngredients => Ingredients?.Split(',');
    }
}