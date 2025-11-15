using MediatR;

namespace Application.UseCases.v1.Destination.Query.GetDestinationById;

public class GetDestinationByIdQuery : IRequest<GetDestinationByIdResponse>
{
    public Guid DestinationId { get; init; }
}
