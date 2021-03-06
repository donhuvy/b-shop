﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catalog.API.Application.Products.Models;
using Catalog.API.Data.Models;
using Data.UnitOfWork.EF.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.MediatR.Exceptions;

namespace Catalog.API.Application.Products.Queries
{
    public class FindProductByIdQuery : IRequest<ProductDto>
    {
        public long Id { get; set; }

        public FindProductByIdQuery(long id)
        {
            Id = id;
        }
    }

    public class FindProductByIdQueryHandler : IRequestHandler<FindProductByIdQuery, ProductDto>
    {
        private readonly IRepository<Product> _repository;

        public FindProductByIdQueryHandler(IRepository<Product> repository)
        {
            _repository = repository;
        }

        public async Task<ProductDto> Handle(FindProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repository.Query()
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CartMaxQuantity = p.CartMaxQuantity,
                    StockQuantity = p.StockQuantity,
                    ImageUri = p.ImageUri,
                    ReviewsCount = p.ReviewsCount,
                    RatingAverage = p.RatingAverage,
                    Description = p.Description,
                    Categories = p.Categories.Select(s => new CategoryDto
                    {
                        Id = s.CategoryId,
                        Name = s.Category.Name
                    }).ToList()
                })
                .SingleOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (product == null)
            {
                throw new NotFoundException(nameof(Product), request.Id);
            }

            return product;
        }
    }
}
