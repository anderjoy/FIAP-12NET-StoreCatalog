using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekBurger.StoreCatalog.WebAPI.Models
{
    public class ProductionAreas
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Status { get; set; }

        public string Restrictions { get; set; }

        [NotMapped]
        public IList<string> ListRestrictions => Restrictions?.Split(',');
    }
}
