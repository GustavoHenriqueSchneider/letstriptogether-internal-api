using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroup.Query.AdminGetGroupById;

public class AdminGetGroupByIdQuery : IRequest<AdminGetGroupByIdResponse>
{
    public Guid GroupId { get; init; }
}
