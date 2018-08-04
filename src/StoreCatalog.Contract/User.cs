using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GeekBurguer.StoreCatalog.Contract
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string StoreNome { get; set; }

        [NotMapped]
        public string[] Restrictions { get; set; }
        public string ListRestrictions
        {
            get { return string.Join(",", Restrictions); }
            set { Restrictions = value.Split(',').ToArray(); }
        }
    }
}
