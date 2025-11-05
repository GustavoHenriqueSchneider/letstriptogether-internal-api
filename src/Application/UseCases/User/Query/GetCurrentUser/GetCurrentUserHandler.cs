using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.User.Query.GetCurrentUser;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, GetCurrentUserResponse>
{
    private readonly IUserRepository _userRepository;

    public GetCurrentUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetCurrentUserResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdWithPreferencesAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }
        
        _ = new UserPreference(user.Preferences);

        return new GetCurrentUserResponse
        {
            Name = user.Name,
            Email = user.Email,
            Preferences = user.Preferences is not null ? 
                new GetCurrentUserPreferenceResponse
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
