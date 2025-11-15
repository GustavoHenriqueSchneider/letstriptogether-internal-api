using FluentValidation;

namespace Application.UseCases.v1.Admin.AdminDestination.Query.AdminGetAllDestinations;

public class AdminGetAllDestinationsValidator : AbstractValidator<AdminGetAllDestinationsQuery>
{
    public AdminGetAllDestinationsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}
