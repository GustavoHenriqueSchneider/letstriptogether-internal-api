using Application.Common.Exceptions;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.UserAggregate;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Common;
using MediatR;

namespace Application.UseCases.User.Command.SetCurrentUserPreferences;

public class SetCurrentUserPreferencesHandler(
    IGroupMemberRepository groupMemberRepository,
    IGroupPreferenceRepository groupPreferenceRepository,
    IGroupRepository groupRepository,
    IUnitOfWork unitOfWork,
    IUserPreferenceRepository userPreferenceRepository,
    IUserRepository userRepository)
    : IRequestHandler<SetCurrentUserPreferencesCommand>
{
    public async Task Handle(SetCurrentUserPreferencesCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }
        
        var preferences = new UserPreference(request.LikesCommercial, request.Food,
            request.Culture, request.Entertainment, request.PlaceTypes);

        user.SetPreferences(preferences);

        userRepository.Update(user);
        await userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);

        var groupMemberships = 
            (await groupMemberRepository.GetAllByUserIdAsync(user.Id, cancellationToken)).ToList();

        foreach (var membership in groupMemberships)
        {
            var group =
                await groupRepository.GetGroupWithMembersPreferencesAsync(membership.GroupId, cancellationToken);

            if (group is null)
            {
                throw new BadRequestException(
                    "Some of the groups that user is member were not found.");
            }
            
            group.UpdatePreferences();
            var groupToUpdate = await groupRepository.GetGroupWithPreferencesAsync(membership.GroupId, cancellationToken);

            if (groupToUpdate is null)
            {
                throw new BadRequestException(
                    "Some of the groups that user is member were not found in the database.");
            }
            
            groupToUpdate.Preferences.Update(group.Preferences);

            groupRepository.Update(groupToUpdate);
            groupPreferenceRepository.Update(groupToUpdate.Preferences);
        }

        if (groupMemberships.Count != 0)
        {
            await unitOfWork.SaveAsync(cancellationToken);
        }
    }
}
