using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroupInvitation.Query.AdminGetGroupInvitationById;

public class AdminGetGroupInvitationByIdHandler : IRequestHandler<AdminGetGroupInvitationByIdQuery, AdminGetGroupInvitationByIdResponse>
{
    private readonly IGroupInvitationRepository _groupInvitationRepository;
    private readonly IGroupRepository _groupRepository;

    public AdminGetGroupInvitationByIdHandler(
        IGroupInvitationRepository groupInvitationRepository,
        IGroupRepository groupRepository)
    {
        _groupInvitationRepository = groupInvitationRepository;
        _groupRepository = groupRepository;
    }

    public async Task<AdminGetGroupInvitationByIdResponse> Handle(AdminGetGroupInvitationByIdQuery request, CancellationToken cancellationToken)
    {
        var groupExists = await _groupRepository.ExistsByIdAsync(request.GroupId, cancellationToken);

        if (!groupExists)
        {
            throw new NotFoundException("Group not found.");
        }

        var invitation = await _groupInvitationRepository.GetByIdAsync(request.InvitationId, cancellationToken);

        if (invitation is null)
        {
            throw new NotFoundException("Group invitation not found.");
        }

        if (invitation.GroupId != request.GroupId)
        {
            throw new BadRequestException("The invitation does not belong to this group.");
        }

        return new AdminGetGroupInvitationByIdResponse
        {
            Status = invitation.Status.ToString(),
            ExpirationDate = invitation.ExpirationDate,
            CreatedAt = invitation.CreatedAt,
            UpdatedAt = invitation.UpdatedAt
        };
    }
}
