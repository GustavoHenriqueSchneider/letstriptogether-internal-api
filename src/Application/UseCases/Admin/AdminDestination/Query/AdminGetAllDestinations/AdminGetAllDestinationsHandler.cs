using Domain.Aggregates.DestinationAggregate;
using MediatR;

namespace Application.UseCases.Admin.AdminDestination.Query.AdminGetAllDestinations;

public class AdminGetAllDestinationsHandler(IDestinationRepository destinationRepository)
    : IRequestHandler<AdminGetAllDestinationsQuery, AdminGetAllDestinationsResponse>
{
    public async Task<AdminGetAllDestinationsResponse> Handle(AdminGetAllDestinationsQuery request, CancellationToken cancellationToken)
    {
        var (destinations, hits) = await destinationRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);

        return new AdminGetAllDestinationsResponse
        {
            Data = destinations.Select(x => new AdminGetAllDestinationsResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        };
    }
}
