using FluentValidation;

namespace Application.UseCases.v1.Destination.Query.GetDestinationById;

public class GetDestinationByIdValidator : AbstractValidator<GetDestinationByIdQuery>
{
    public GetDestinationByIdValidator()
    {
        RuleFor(x => x.DestinationId)
            .NotEmpty();
    }
}
