using Application.Common.Exceptions;
using Domain.Aggregates.UserAggregate;
using Domain.Aggregates.UserAggregate.Entities;
using MediatR;

namespace Application.UseCases.v1.Admin.AdminUser.Query.AdminGetUserById;

public class AdminGetUserByIdHandler(IUserRepository userRepository)
    : IRequestHandler<AdminGetUserByIdQuery, AdminGetUserByIdResponse>
{
    public async Task<AdminGetUserByIdResponse> Handle(AdminGetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }
        
        _ = new UserPreference(user.Preferences);

        return new AdminGetUserByIdResponse
        {
            Name = user.Name,
            Email = user.Email.ToLowerInvariant(),
            Preferences = user.Preferences is not null ?
                new AdminGetUserByIdPreferenceResponse
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
