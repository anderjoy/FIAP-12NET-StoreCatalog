using System;

namespace GeekBurger.StoreCatalog.Contract.Model
{
    public class StoreCatalogReady
    {
        public Guid Id { get; set; }
        public bool IsReady { get; set; }
    }
}
