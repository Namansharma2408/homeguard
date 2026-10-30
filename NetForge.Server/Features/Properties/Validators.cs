using FluentValidation;

namespace NetForge.Server.Features.Properties;

internal sealed class CreatePropertyValidator : AbstractValidator<CreatePropertyRequest>
{
    public CreatePropertyValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PropertyType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(300);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.State).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PostalCode).MaximumLength(20);
        RuleFor(x => x.YearBuilt).InclusiveBetween(1800, 2100).When(x => x.YearBuilt.HasValue);
        RuleFor(x => x.Rooms).GreaterThanOrEqualTo(0).When(x => x.Rooms.HasValue);
        RuleFor(x => x.SquareFeet).GreaterThanOrEqualTo(0).When(x => x.SquareFeet.HasValue);
    }
}

internal sealed class UpdatePropertyValidator : AbstractValidator<UpdatePropertyRequest>
{
    public UpdatePropertyValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PropertyType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(300);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.State).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PostalCode).MaximumLength(20);
        RuleFor(x => x.YearBuilt).InclusiveBetween(1800, 2100).When(x => x.YearBuilt.HasValue);
        RuleFor(x => x.Rooms).GreaterThanOrEqualTo(0).When(x => x.Rooms.HasValue);
        RuleFor(x => x.SquareFeet).GreaterThanOrEqualTo(0).When(x => x.SquareFeet.HasValue);
    }
}