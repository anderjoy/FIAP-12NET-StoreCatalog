using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
