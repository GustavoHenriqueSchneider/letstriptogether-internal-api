using MediatR;

namespace Application.UseCases.Admin.AdminDestination.Query.AdminGetAllDestinations;

public class AdminGetAllDestinationsQuery : IRequest<AdminGetAllDestinationsResponse>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
