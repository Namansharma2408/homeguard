using FluentValidation;

namespace NetForge.Server.Features.Incidents;

internal sealed class CreateIncidentValidator : AbstractValidator<CreateIncidentRequest>
{
    public CreateIncidentValidator()
    {
        RuleFor(x => x.PropertyId).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Priority).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Description).MaximumLength(2000);
    }
}