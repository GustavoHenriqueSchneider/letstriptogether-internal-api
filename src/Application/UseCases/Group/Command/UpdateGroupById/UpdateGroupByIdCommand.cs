using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Command.UpdateGroupById;

public class UpdateGroupByIdCommand : IRequest
{
    public Guid GroupId { get; init; }
    public Guid UserId { get; init; }
    public string? Name { get; init; }
    public DateTime? TripExpectedDate { get; init; }
}
