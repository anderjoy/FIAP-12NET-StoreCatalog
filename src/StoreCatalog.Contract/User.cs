using System;

namespace GeekBurguer.StoreCatalog.Contract
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string StoreNome { get; set; }
        public string[] Restrictions { get; set; }
    }
}
