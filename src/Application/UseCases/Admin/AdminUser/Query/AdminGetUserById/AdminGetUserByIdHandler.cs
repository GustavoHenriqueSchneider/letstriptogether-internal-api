using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;

using MediatR;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Query.AdminGetUserById;

public class AdminGetUserByIdHandler : IRequestHandler<AdminGetUserByIdQuery, AdminGetUserByIdResponse>
{
    private readonly IUserRepository _userRepository;

    public AdminGetUserByIdHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AdminGetUserByIdResponse> Handle(AdminGetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdWithPreferencesAsync(request.UserId, cancellationToken);

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
