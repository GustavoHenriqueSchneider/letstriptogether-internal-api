using LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminDestination.Query.AdminGetAllDestinations;

public class AdminGetAllDestinationsHandler : IRequestHandler<AdminGetAllDestinationsQuery, AdminGetAllDestinationsResponse>
{
    private readonly IDestinationRepository _destinationRepository;

    public AdminGetAllDestinationsHandler(IDestinationRepository destinationRepository)
    {
        _destinationRepository = destinationRepository;
    }

    public async Task<AdminGetAllDestinationsResponse> Handle(AdminGetAllDestinationsQuery request, CancellationToken cancellationToken)
    {
        var (destinations, hits) = await _destinationRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);

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
