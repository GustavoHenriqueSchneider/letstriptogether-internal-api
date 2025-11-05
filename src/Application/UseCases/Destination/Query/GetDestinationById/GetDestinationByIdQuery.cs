using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Destination.Query.GetDestinationById;

public class GetDestinationByIdQuery : IRequest<GetDestinationByIdResponse>
{
    public Guid DestinationId { get; init; }
}
