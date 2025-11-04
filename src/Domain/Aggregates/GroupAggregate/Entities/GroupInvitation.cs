using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Enums;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;

public class GroupInvitation : TrackableEntity
{
    public Guid GroupId { get; init; }
    public Group Group { get; init; } = null!;

    private DateTime _expirationDate = DateTime.UtcNow.AddDays(7);
    public DateTime ExpirationDate
    {
        get => _expirationDate.ToUniversalTime();
        private set => _expirationDate = value;
    }

    public GroupInvitationStatus Status { get; private set; } = GroupInvitationStatus.Active;

    private readonly List<UserGroupInvitation> _answeredBy = [];
    public IReadOnlyCollection<UserGroupInvitation> AnsweredBy => _answeredBy.AsReadOnly();

    private GroupInvitation() { }
    
    public GroupInvitation(Guid groupId)
    {
        GroupId = groupId;
    }

    public void Expire()
    {
        _ = Status switch
        {
            GroupInvitationStatus.Active => Status = GroupInvitationStatus.Expired,
            GroupInvitationStatus.Cancelled => 
                throw new InvalidOperationException("It is not possible to expire a cancelled group invitation."),
            GroupInvitationStatus.Expired => 
                throw new InvalidOperationException("It is not possible to expire a already expired group invitation."),
            _ => throw new ArgumentOutOfRangeException($"Invalid status {Status} for group invitation")
        };
    }
    
    public void Cancel()
    {
        _ = Status switch
        {
            GroupInvitationStatus.Active => Status = GroupInvitationStatus.Cancelled,
            GroupInvitationStatus.Cancelled => 
                throw new InvalidOperationException("It is not possible to cancel a already cancelled group invitation."),
            GroupInvitationStatus.Expired => 
                throw new InvalidOperationException("It is not possible to cancel a expired group invitation."),
            _ => throw new ArgumentOutOfRangeException($"Invalid status {Status} for group invitation")
        };
    }
    
    public GroupInvitation Copy()
    {
        Cancel();
        return new GroupInvitation(GroupId);
    }
    
    private bool HasAnswer(UserGroupInvitation answer)
    {
        return _answeredBy.Contains(answer);
    }

    public UserGroupInvitation AddAnswer(Guid userId, bool isAccepted)
    {
        var answer = new UserGroupInvitation
        {
            GroupInvitationId = Id,
            UserId = userId,
            IsAccepted = isAccepted
        };
        
        if (HasAnswer(answer))
        {
            throw new InvalidOperationException("This answer is already included on the invitation.");
        }

        _answeredBy.Add(answer);
        return answer;
    }
}