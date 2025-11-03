using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Requests.Group;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Destination;
using WebApi.DTOs.Responses.Group;
using WebApi.Models.Aggregates;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;

namespace WebApi.Controllers;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Authorize]
[Route("api/v1/groups")]
public class GroupController(
    IGroupRepository groupRepository,
    IApplicationUserContext currentUser,
    IUserRepository userRepository,
    IGroupPreferenceRepository groupPreferenceRepository,
    IGroupMemberRepository groupMemberRepository,
    IDestinationRepository destinationRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
    {
        var currentUserId = currentUser.GetId();
        var user = await userRepository.GetByIdWithPreferencesAsync(currentUserId);

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

            await groupRepository.AddAsync(group);
            await groupMemberRepository.AddAsync(groupMember);
            await groupPreferenceRepository.AddAsync(groupPreferences);

            await unitOfWork.SaveAsync();
            return CreatedAtAction(nameof(CreateGroup), new CreateGroupResponse { Id = group.Id });
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("Invalid"))
        {
            return UnprocessableEntity(new ErrorResponse(ex.Message));
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGroups([FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var (groups, hits) = await groupRepository.GetAllGroupsByUserIdAsync(
            currentUser.GetId(), pageNumber, pageSize);

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
    public async Task<IActionResult> GetGroupById([FromRoute] Guid groupId)
    {
        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

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
            var groupPreferences = await groupPreferenceRepository.GetByGroupIdAsync(groupId)
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
        [FromBody] UpdateGroupRequest request)
    {
        var currentUserId = currentUser.GetId();

        if (!await userRepository.ExistsByIdAsync(currentUserId))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

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
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    [HttpDelete("{groupId:guid}")]
    public async Task<IActionResult> DeleteGroupById([FromRoute] Guid groupId)
    {
        var currentUserId = currentUser.GetId();

        if (!await userRepository.ExistsByIdAsync(currentUserId))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

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
        await unitOfWork.SaveAsync();

        return NoContent();
    }
    
    [HttpPatch("{groupId:guid}/leave")]
    public async Task<IActionResult> LeaveGroupById([FromRoute] Guid groupId)
    {
        var currentUserId = currentUser.GetId();
        if (!await userRepository.ExistsByIdAsync(currentUserId))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMembersPreferencesAsync(groupId);
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
        
        groupRepository.Update(group);
        groupMemberRepository.Remove(currentUserMember);
        groupPreferenceRepository.Update(group.Preferences);
        
        await unitOfWork.SaveAsync();
        return NoContent();
    }
    
    [HttpGet("{groupId:guid}/destinations-not-voted")]
    public async Task<IActionResult> GetNotVotedDestinationsByMemberOnGroup([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var currentUserId = currentUser.GetId();
        if (!await userRepository.ExistsByIdAsync(currentUserId))
        {
            return NotFound(new ErrorResponse("User not found."));
        }
        
        var group = await groupRepository.GetGroupWithMembersAsync(groupId);
        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }
        
        var isGroupMember = group.Members.Any(m => m.UserId == currentUserId);
        if (!isGroupMember)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }
        
        var (destinations, hits) = await destinationRepository.GetNotVotedByUserInGroupAsync(
            currentUserId, groupId, pageNumber, pageSize);

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

