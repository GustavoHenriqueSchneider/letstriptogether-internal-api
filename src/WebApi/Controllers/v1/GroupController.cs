using LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;
using LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Requests.Group;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.Destination;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/groups")]
public class GroupController(
    IGroupRepository groupRepository,
    IApplicationUserContextExtensions currentUser,
    IUserRepository userRepository,
    IGroupPreferenceRepository groupPreferenceRepository,
    IGroupMemberRepository groupMemberRepository,
    IDestinationRepository destinationRepository,
    IGroupMatchRepository groupMatchRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetId();
        var user = await userRepository.GetByIdWithPreferencesAsync(currentUserId, cancellationToken);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        if (user.Preferences is null)
        {
            return BadRequest(new ErrorResponse("User has not filled any preferences yet."));
        }

        try
        {
            _ = new UserPreference(user.Preferences);
            var group = new Group(request.Name, request.TripExpectedDate.ToUniversalTime());

            var groupMember = group.AddMember(user, isOwner: true);
            var groupPreferences = group.UpdatePreferences(user.Preferences);

            await groupRepository.AddAsync(group, cancellationToken);
            await groupMemberRepository.AddAsync(groupMember, cancellationToken);
            await groupPreferenceRepository.AddAsync(groupPreferences, cancellationToken);

            await unitOfWork.SaveAsync(cancellationToken);
            return CreatedAtAction(nameof(CreateGroup), new CreateGroupResponse { Id = group.Id });
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("Invalid"))
        {
            return UnprocessableEntity(new ErrorResponse(ex.Message));
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGroups([FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var (groups, hits) = await groupRepository.GetAllGroupsByUserIdAsync(
            currentUser.GetId(), pageNumber, pageSize, cancellationToken);

        return Ok(new GetAllGroupsResponse
        {
            Data = groups.Select(x => new GetAllGroupsResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }

    [HttpGet("{groupId:guid}")]
    public async Task<IActionResult> GetGroupById([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetGroupWithMembersAsync(groupId, cancellationToken);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var isMember = group.Members.Any(x => x.UserId == currentUser.GetId());

        if (!isMember)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        try
        {
            var groupPreferences = await groupPreferenceRepository.GetByGroupIdAsync(groupId, cancellationToken)
                ?? throw new InvalidOperationException("Invalid preferences");

            return Ok(new GetGroupByIdResponse
            {
                Name = group.Name,
                TripExpectedDate = group.TripExpectedDate,
                Preferences = new GetGroupByIdPreferenceResponse
                {
                    LikesCommercial = groupPreferences.LikesCommercial,
                    Food = groupPreferences.Food,
                    Culture = groupPreferences.Culture,
                    Entertainment = groupPreferences.Entertainment,
                    PlaceTypes = groupPreferences.PlaceTypes,
                },
                CreatedAt = group.CreatedAt,
                UpdatedAt = group.UpdatedAt
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("Invalid"))
        {
            return UnprocessableEntity(
                new ErrorResponse("Invalid preferences are filled for this group, please contact support."));
        }
    }

    [HttpPut("{groupId:guid}")]
    public async Task<IActionResult> UpdateGroupById([FromRoute] Guid groupId, 
        [FromBody] UpdateGroupRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetId();

        if (!await userRepository.ExistsByIdAsync(currentUserId, cancellationToken))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupId, cancellationToken);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var currentUserMember = group.Members.SingleOrDefault(m => m.UserId == currentUserId);

        if (currentUserMember is null)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        if (!currentUserMember.IsOwner)
        {
            return BadRequest(new ErrorResponse("Only the group owner can update group data."));
        }

        group.Update(request.Name, request.TripExpectedDate);
        groupRepository.Update(group);
        await unitOfWork.SaveAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{groupId:guid}")]
    public async Task<IActionResult> DeleteGroupById([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetId();

        if (!await userRepository.ExistsByIdAsync(currentUserId, cancellationToken))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupId, cancellationToken);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var currentUserMember = group.Members.SingleOrDefault(m => m.UserId == currentUserId);

        if (currentUserMember is null)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        if (!currentUserMember.IsOwner)
        {
            return BadRequest(new ErrorResponse("Only the group owner can delete the group."));
        }

        groupRepository.Remove(group);
        await unitOfWork.SaveAsync(cancellationToken);

        return NoContent();
    }
    
    [HttpPatch("{groupId:guid}/leave")]
    public async Task<IActionResult> LeaveGroupById([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetId();
        if (!await userRepository.ExistsByIdAsync(currentUserId, cancellationToken))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMembersPreferencesAsync(groupId, cancellationToken);
        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var currentUserMember = group.Members.SingleOrDefault(m => m.UserId == currentUserId);
        if (currentUserMember is null)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        if (currentUserMember.IsOwner)
        {
            return BadRequest(new ErrorResponse("The group owner can not leave the group, only delete it."));
        }

        group.RemoveMember(currentUserMember);
        
        if (group.Members.Count == 1)
        {
            var matches = await groupMatchRepository.GetAllMatchesByGroupAsync(groupId, cancellationToken);
            groupMatchRepository.RemoveRange(matches);
        }
        
        groupRepository.Update(group);
        groupMemberRepository.Remove(currentUserMember);
        groupPreferenceRepository.Update(group.Preferences);
        
        await unitOfWork.SaveAsync(cancellationToken);
        return NoContent();
    }
    
    [HttpGet("{groupId:guid}/destinations-not-voted")]
    public async Task<IActionResult> GetNotVotedDestinationsByMemberOnGroup([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUser.GetId();
        if (!await userRepository.ExistsByIdAsync(currentUserId, cancellationToken))
        {
            return NotFound(new ErrorResponse("User not found."));
        }
        
        var group = await groupRepository.GetGroupWithMembersPreferencesAsync(groupId, cancellationToken);
        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }
        
        var isGroupMember = group.Members.Any(m => m.UserId == currentUserId);
        if (!isGroupMember)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }
        
        var groupPreferences = group.Preferences.ToList();
        
        var (destinations, hits) = await destinationRepository.GetNotVotedByUserInGroupAsync(
            currentUserId, groupId, groupPreferences, pageNumber, pageSize, cancellationToken);

        return Ok(new GetAllNotVotedDestinationsForGroupResponse
        {
            Data = destinations.Select(x => new GetAllNotVotedDestinationsForGroupResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }
}

