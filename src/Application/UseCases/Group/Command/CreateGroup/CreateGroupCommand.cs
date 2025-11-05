using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Command.CreateGroup;

public class CreateGroupCommand : IRequest<CreateGroupResponse>
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = null!;
    public DateTime TripExpectedDate { get; init; }
}
