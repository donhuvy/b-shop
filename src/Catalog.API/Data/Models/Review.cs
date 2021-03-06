﻿using System.Collections.Generic;
using Catalog.API.Data.Models.Enums;
using Data.Entities.Models;

namespace Catalog.API.Data.Models
{
    public class Review : AuditEntity
    {
        public Review()
        {
            ReviewStatus = ReviewStatus.Pending;
        }

        public string CreatedById { get; set; }

        public string Title { get; set; }

        public string Comment { get; set; }

        public int Rating { get; set; }

        public ReviewStatus ReviewStatus { get; set; }

        public IList<Reply> Replies { get; protected set; } = new List<Reply>();

        public long ProductId { get; set; }

        public Product Product { get; set; }
    }
}
