using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Query.AdminGetUserById;

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
            Email = user.Email,
            Preferences = user.Preferences is not null ?
                new AdminGetUserByIdPreferenceResponse
                {
                    LikesCommercial = user.Preferences.LikesCommercial,
                    Food = user.Preferences.Food,
                    Culture = user.Preferences.Culture,
                    Entertainment = user.Preferences.Entertainment,
                    PlaceTypes = user.Preferences.PlaceTypes,
                } : null,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
