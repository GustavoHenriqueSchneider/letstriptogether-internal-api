using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminSetUserPreferencesByUserId;

public class AdminSetUserPreferencesByUserIdHandler : IRequestHandler<AdminSetUserPreferencesByUserIdCommand>
{
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IGroupPreferenceRepository _groupPreferenceRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserPreferenceRepository _userPreferenceRepository;
    private readonly IUserRepository _userRepository;

    public AdminSetUserPreferencesByUserIdHandler(
        IGroupMemberRepository groupMemberRepository,
        IGroupPreferenceRepository groupPreferenceRepository,
        IGroupRepository groupRepository,
        IUnitOfWork unitOfWork,
        IUserPreferenceRepository userPreferenceRepository,
        IUserRepository userRepository)
    {
        _groupMemberRepository = groupMemberRepository;
        _groupPreferenceRepository = groupPreferenceRepository;
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
        _userPreferenceRepository = userPreferenceRepository;
        _userRepository = userRepository;
    }

    public async Task Handle(AdminSetUserPreferencesByUserIdCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdWithPreferencesAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }
        
        var preferences = new UserPreference(request.LikesCommercial, request.Food,
            request.Culture, request.Entertainment, request.PlaceTypes);

        user.SetPreferences(preferences);

        _userRepository.Update(user);
        await _userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        var groupMemberships = 
            (await _groupMemberRepository.GetAllByUserIdAsync(user.Id, cancellationToken)).ToList();

        foreach (var membership in groupMemberships)
        {
            var group =
                await _groupRepository.GetGroupWithMembersPreferencesAsync(membership.GroupId, cancellationToken);

            if (group is null)
            {
                throw new BadRequestException(
                    "Some of the groups that user is member were not found.");
            }
            
            group.UpdatePreferences();
            var groupToUpdate = await _groupRepository.GetGroupWithPreferencesAsync(membership.GroupId, cancellationToken);

            if (groupToUpdate is null)
            {
                throw new BadRequestException(
                    "Some of the groups that user is member were not found in the database.");
            }
            
            groupToUpdate.Preferences.Update(group.Preferences);

            _groupRepository.Update(groupToUpdate);
            _groupPreferenceRepository.Update(groupToUpdate.Preferences);
        }

        if (groupMemberships.Count != 0)
        {
            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }
}
