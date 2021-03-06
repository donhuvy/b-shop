﻿using System.Threading;
using System.Threading.Tasks;
using Catalog.API.Application.Categories.Models;
using Catalog.API.Data.Models;
using Data.UnitOfWork.EF.Core;
using FluentValidation;
using MediatR;

namespace Catalog.API.Application.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<CategoryDto>
    {
        public string Name { get; set; }
    }

    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(s => s.Name)
                .NotEmpty()
                .MinimumLength(5);
        }
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Category> _repository;

        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.Repository<Category>();
        }

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Category
            {
                Name = request.Name
            };

            await _repository.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }
    }
}
