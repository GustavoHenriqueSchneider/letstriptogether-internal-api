using MediatR;

namespace Application.UseCases.v1.Admin.AdminGroup.Query.AdminGetGroupById;

public class AdminGetGroupByIdQuery : IRequest<AdminGetGroupByIdResponse>
{
    public Guid GroupId { get; init; }
}
