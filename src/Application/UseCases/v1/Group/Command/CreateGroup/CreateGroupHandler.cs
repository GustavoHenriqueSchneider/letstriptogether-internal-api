using Application.Common.Exceptions;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.UserAggregate;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Common;
using MediatR;
using GroupModel = Domain.Aggregates.GroupAggregate.Entities.Group;

namespace Application.UseCases.v1.Group.Command.CreateGroup;

public class CreateGroupHandler(
    IGroupMemberRepository groupMemberRepository,
    IGroupPreferenceRepository groupPreferenceRepository,
    IGroupRepository groupRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository)
    : IRequestHandler<CreateGroupCommand, CreateGroupResponse>
{
    public async Task<CreateGroupResponse> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        var user = await userRepository.GetByIdWithPreferencesAsync(currentUserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        if (user.Preferences is null)
        {
            throw new BadRequestException("User has not filled any preferences yet.");
        }
        
        _ = new UserPreference(user.Preferences);
        var group = new GroupModel(request.Name, request.TripExpectedDate.ToUniversalTime());

        var groupMember = group.AddMember(user, isOwner: true);
        var groupPreferences = group.UpdatePreferences(user.Preferences);

        await groupRepository.AddAsync(group, cancellationToken);
        await groupMemberRepository.AddAsync(groupMember, cancellationToken);
        await groupPreferenceRepository.AddAsync(groupPreferences, cancellationToken);

        await unitOfWork.SaveAsync(cancellationToken);
        return new CreateGroupResponse { Id = group.Id };
    }
}
