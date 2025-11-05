using LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate;
using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.Destination.Query.GetDestinationById;

public class GetDestinationByIdHandler(IDestinationRepository destinationRepository)
    : IRequestHandler<GetDestinationByIdQuery, GetDestinationByIdResponse>
{
    public async Task<GetDestinationByIdResponse> Handle(GetDestinationByIdQuery request, CancellationToken cancellationToken)
    {
        var destination = await destinationRepository.GetByIdAsync(request.DestinationId, cancellationToken);

        if (destination is null)
        {
            throw new NotFoundException("Destination not found.");
        }

        return new GetDestinationByIdResponse
        {
            Place = destination.Address,
            Description = destination.Description,
            Attractions = destination.Attractions.Select(a => new DestinationAttractionModel
            {
                Name = a.Name,
                Description = a.Description,
                Category = a.Category
            }),
            CreatedAt = destination.CreatedAt,
            UpdatedAt = destination.UpdatedAt
        };
    }
}
