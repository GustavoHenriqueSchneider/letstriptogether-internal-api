using MediatR;

namespace Application.UseCases.Admin.AdminUser.Command.AdminSetUserPreferencesByUserId;

public record AdminSetUserPreferencesByUserIdCommand : IRequest
{
    public Guid UserId { get; init; }
    public bool LikesShopping { get; init; }
    public bool LikesGastronomy { get; init; }
    public List<string> Culture { get; init; } = [];
    public List<string> Entertainment { get; init; } = [];
    public List<string> PlaceTypes { get; init; } = [];
}
