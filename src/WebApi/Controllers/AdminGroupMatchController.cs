using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Requests.GroupMatch;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.GroupMatch;
using WebApi.Models;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Controllers;

[Authorize(Policy = Policies.Admin)]
[ApiController]
[Route("api/v1/admin/group-matches")]
public class AdminGroupMatchController(
    IGroupMatchRepository groupMatchRepository,
    IGroupRepository groupRepository,
    IDestinationRepository destinationRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var (groupMatches, hits) = await groupMatchRepository.GetAllAsync(pageNumber, pageSize);

        return Ok(new GetAllGroupMatchesResponse
        {
            Data = MapToResponseData(groupMatches),
            Hits = hits
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var groupMatch = await groupMatchRepository.GetByIdWithRelationsAsync(id);

        if (groupMatch is null)
        {
            return NotFound(new ErrorResponse("Group match not found."));
        }

        return Ok(new GetGroupMatchByIdResponse
        {
            Id = groupMatch.Id,
            GroupId = groupMatch.GroupId,
            GroupName = groupMatch.Group.Name,
            DestinationId = groupMatch.DestinationId,
            DestinationAddress = groupMatch.Destination.Address,
            DestinationCategories = groupMatch.Destination.Categories,
            CreatedAt = groupMatch.CreatedAt
        });
    }

    [HttpGet("by-group/{groupId:guid}")]
    public async Task<IActionResult> GetByGroupId([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var (groupMatches, hits) = await groupMatchRepository.GetByGroupIdAsync(groupId, pageNumber, pageSize);

        return Ok(new GetAllGroupMatchesResponse
        {
            Data = MapToResponseData(groupMatches),
            Hits = hits
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupMatchRequest request)
    {
        var groupExists = await groupRepository.ExistsByIdAsync(request.GroupId);
        if (!groupExists)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var destinationExists = await destinationRepository.ExistsByIdAsync(request.DestinationId);
        if (!destinationExists)
        {
            return NotFound(new ErrorResponse("Destination not found."));
        }

        var groupMatch = new GroupMatch(request.GroupId, request.DestinationId);

        await groupMatchRepository.AddAsync(groupMatch);
        await unitOfWork.SaveAsync();

        return CreatedAtAction(nameof(Create), new CreateGroupMatchResponse { Id = groupMatch.Id });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteById([FromRoute] Guid id)
    {
        var groupMatch = await groupMatchRepository.GetByIdAsync(id);

        if (groupMatch is null)
        {
            return NotFound(new ErrorResponse("Group match not found."));
        }

        groupMatchRepository.Remove(groupMatch);
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    private static IEnumerable<GetAllGroupMatchesResponseData> MapToResponseData(IEnumerable<GroupMatch> groupMatches)
    {
        return groupMatches.Select(x => new GetAllGroupMatchesResponseData
        {
            Id = x.Id,
            GroupId = x.GroupId,
            GroupName = x.Group.Name,
            DestinationId = x.DestinationId,
            DestinationAddress = x.Destination.Address,
            DestinationCategories = x.Destination.Categories,
            CreatedAt = x.CreatedAt
        });
    }
}
