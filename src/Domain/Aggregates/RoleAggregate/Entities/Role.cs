using Domain.Aggregates.UserAggregate.Entities;
using Domain.Common;

namespace Domain.Aggregates.RoleAggregate.Entities;

public class Role : TrackableEntity
{
    public string Name { get; init; } = null!;

    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    public Role() { }
}
