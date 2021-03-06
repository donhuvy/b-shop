﻿using System.Collections.Generic;
using Data.Entities.Models;

namespace Catalog.API.Data.Models
{
    public class Category : AuditEntity
    {
        public string Name { get; set; }

        public IList<ProductCategory> Products { get; protected set; } = new List<ProductCategory>();
    }
}
