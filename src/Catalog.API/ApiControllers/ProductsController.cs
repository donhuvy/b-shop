﻿using System.Threading;
using System.Threading.Tasks;
using Catalog.API.Application.Products.Commands;
using Catalog.API.Application.Products.Models;
using Catalog.API.Application.Products.Queries;
using Data.Entities.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.ApiControllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ResponseCache(Duration = 5)]
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<OffsetPaged<ProductDto>>> FindProducts([FromQuery] FindProductsQuery request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return result;
        }

        [ResponseCache(Duration = 5)]
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> FindProductById(long id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new FindProductByIdQuery(id), cancellationToken);

            return result;
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromForm] CreateProductCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return CreatedAtAction(nameof(FindProductById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(long id, [FromForm] UpdateProductCommand request, CancellationToken cancellationToken)
        {
            request.Id = id;
            await _mediator.Send(request, cancellationToken);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteProductCommand(id), cancellationToken);

            return Ok();
        }
    }
}
