using Application.Common.Exceptions;
using Domain.Aggregates.GroupAggregate;
using MediatR;

namespace Application.UseCases.Admin.AdminGroupInvitation.Query.AdminGetGroupInvitationById;

public class AdminGetGroupInvitationByIdHandler(
    IGroupInvitationRepository groupInvitationRepository,
    IGroupRepository groupRepository)
    : IRequestHandler<AdminGetGroupInvitationByIdQuery, AdminGetGroupInvitationByIdResponse>
{
    public async Task<AdminGetGroupInvitationByIdResponse> Handle(AdminGetGroupInvitationByIdQuery request, CancellationToken cancellationToken)
    {
        var groupExists = await groupRepository.ExistsByIdAsync(request.GroupId, cancellationToken);

        if (!groupExists)
        {
            throw new NotFoundException("Group not found.");
        }

        var invitation = await groupInvitationRepository.GetByIdAsync(request.InvitationId, cancellationToken);

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
