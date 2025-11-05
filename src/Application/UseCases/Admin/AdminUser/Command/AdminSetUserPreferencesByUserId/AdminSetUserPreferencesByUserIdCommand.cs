using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Command.AdminSetUserPreferencesByUserId;

public class AdminSetUserPreferencesByUserIdCommand : IRequest
{
    public Guid UserId { get; init; }
    public bool LikesCommercial { get; init; }
    public List<string> Food { get; init; } = [];
    public List<string> Culture { get; init; } = [];
    public List<string> Entertainment { get; init; } = [];
    public List<string> PlaceTypes { get; init; } = [];
}
