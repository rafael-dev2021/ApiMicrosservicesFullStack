using ApiMicrosservicesProduct.DTOs;
using FluentValidation;

namespace ApiMicrosservicesProduct.FluentValidation.ProductDtoValidationLibrary;

public class ProductDtoValidator : AbstractValidator<ProductDto>
{
    public ProductDtoValidator()
    {
        RuleFor(product => product.Name)
            .NotEmpty().WithMessage("Product name is required")
            .Length(3, 100).WithMessage("Name should have between 3 and 100 characters");

        RuleFor(product => product.Description)
            .NotEmpty().WithMessage("Description is required")
            .Length(10, 10000).WithMessage("Description should have between 10 and 10000 characters");

        RuleForEach(product => product.Images)
            .Must(image => image.Length <= 600)
            .WithMessage("Each image should have a maximum length of 600 characters");

        RuleFor(product => product.Price)
            .NotEmpty().WithMessage("Price is required")
            .InclusiveBetween(1, 9999).WithMessage("Price should be between 1 and 9999");

        RuleFor(product => product.Stock)
            .NotEmpty().WithMessage("Stock is required")
            .InclusiveBetween(1, 9999).WithMessage("Stock should be between 1 and 9999");
    }
}
