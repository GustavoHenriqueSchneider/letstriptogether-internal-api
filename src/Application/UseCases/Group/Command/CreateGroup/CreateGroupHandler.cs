using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;
using MediatR;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using GroupModel = LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Command.CreateGroup;

public class CreateGroupHandler : IRequestHandler<CreateGroupCommand, CreateGroupResponse>
{
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IGroupPreferenceRepository _groupPreferenceRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public CreateGroupHandler(
        IGroupMemberRepository groupMemberRepository,
        IGroupPreferenceRepository groupPreferenceRepository,
        IGroupRepository groupRepository,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _groupMemberRepository = groupMemberRepository;
        _groupPreferenceRepository = groupPreferenceRepository;
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task<CreateGroupResponse> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        var user = await _userRepository.GetByIdWithPreferencesAsync(currentUserId, cancellationToken);

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

        await _groupRepository.AddAsync(group, cancellationToken);
        await _groupMemberRepository.AddAsync(groupMember, cancellationToken);
        await _groupPreferenceRepository.AddAsync(groupPreferences, cancellationToken);

        await _unitOfWork.SaveAsync(cancellationToken);
        return new CreateGroupResponse { Id = group.Id };
    }
}
