using System;

namespace StoreCatalog.WebAPI.Models
{
    public class ProductionAreas
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Status { get; set; }

        public string[] Restrictions { get; set; }
    }
}
