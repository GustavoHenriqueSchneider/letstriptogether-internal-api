using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Destination.Query.GetDestinationById;

public class GetDestinationByIdValidator : AbstractValidator<GetDestinationByIdQuery>
{
    public GetDestinationByIdValidator()
    {
        RuleFor(x => x.DestinationId)
            .NotEmpty().WithMessage("DestinationId is required");
    }
}
