namespace WebApi.Models;

public class Role : TrackableEntity
{
    public string Name { get; init; } = null!;

    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private Role() { }
}
