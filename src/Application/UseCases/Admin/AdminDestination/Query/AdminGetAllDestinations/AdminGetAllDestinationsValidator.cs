using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminDestination.Query.AdminGetAllDestinations;

public class AdminGetAllDestinationsValidator : AbstractValidator<AdminGetAllDestinationsQuery>
{
    public AdminGetAllDestinationsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0");
    }
}
