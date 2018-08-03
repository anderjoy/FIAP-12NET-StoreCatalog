﻿using System;
using System.Collections.Generic;

namespace StoreCatalog.WebAPI.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        public virtual IEnumerable<Item> Items { get; set; }
    }
}
