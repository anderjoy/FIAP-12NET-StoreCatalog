using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GeekBurguer.StoreCatalog.Contract
{
    public class ProductionAreas
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Status { get; set; }

        [NotMapped]
        public string[] Restrictions { get; set; }

        public string ListRestrictions
        {
            get { return string.Join(",", Restrictions); }
            set { Restrictions = value.Split(',').ToArray(); }
        }

        public static IEnumerable<ProductionAreas> GetProductionsAreas(string json)
        {
            return JsonConvert.DeserializeObject<IEnumerable<ProductionAreas>>(json);
        }
    }
}
