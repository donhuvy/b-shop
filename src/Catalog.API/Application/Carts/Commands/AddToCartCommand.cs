﻿using System;
using System.Linq;
using Catalog.API.Data.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Catalog.API.Services;
using Data.UnitOfWork.EF.Core;
using Shared.MediatR.Exceptions;

namespace Catalog.API.Application.Carts.Commands
{
    public class AddToCartCommand : IRequest<long>
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
    {
        public AddToCartCommandValidator()
        {
            RuleFor(s => s.Quantity).GreaterThan(0);
        }
    }

    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, long>
    {
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Product> _productRepository;

        public AddToCartCommandHandler(IIdentityService identityService, IUnitOfWork unitOfWork, IRepository<Cart> cartRepository, IRepository<Product> productRepository)
        {
            _identityService = identityService;
            _unitOfWork = unitOfWork;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<long> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var customerId = _identityService.GetCurrentUserId();

            var cart = await _cartRepository.Query()
                .Include(s => s.Items)
                .FindActiveCart(customerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (cart == null)
            {
                throw new NotFoundException();
            }

            if (cart.LockedOnCheckout)
            {
                throw new Exception("Cart is being locked for checkout. Please complete the checkout first");
            }

            var cartItem = cart.Items.FirstOrDefault(s => s.ProductId == request.ProductId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    Cart = cart,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };

                cart.Items.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += request.Quantity;
            }

            var product = await _productRepository.FindAsync(request.ProductId, cancellationToken);

            if (cartItem.Quantity > product.CartMaxQuantity)
            {
                throw new Exception($"Can only add {product.CartMaxQuantity} items per cart");
            }

            if (cartItem.Quantity > product.StockQuantity)
            {
                throw new Exception($"There are only {product.StockQuantity} items available for {product.Name}");
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return cartItem.Id;
        }
    }
}
