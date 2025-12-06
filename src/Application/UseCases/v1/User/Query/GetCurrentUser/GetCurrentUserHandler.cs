using Application.Common.Exceptions;
using Domain.Aggregates.UserAggregate;
using Domain.Aggregates.UserAggregate.Entities;
using MediatR;

namespace Application.UseCases.v1.User.Query.GetCurrentUser;

public class GetCurrentUserHandler(IUserRepository userRepository)
    : IRequestHandler<GetCurrentUserQuery, GetCurrentUserResponse>
{
    public async Task<GetCurrentUserResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }
        
        _ = new UserPreference(user.Preferences);

        return new GetCurrentUserResponse
        {
            Name = user.Name,
            Email = user.Email.ToLowerInvariant(),
            Preferences = user.Preferences is not null ? 
                new GetCurrentUserPreferenceResponse
                {
                    LikesShopping = user.Preferences.LikesShopping,
                    LikesGastronomy = user.Preferences.LikesGastronomy,
                    Culture = user.Preferences.Culture,
                    Entertainment = user.Preferences.Entertainment,
                    PlaceTypes = user.Preferences.PlaceTypes,
                } : null,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
