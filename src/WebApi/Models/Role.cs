namespace WebApi.Models;

public class Role : TrackableEntity
{
    public string Name { get; init; } = null!;
    public List<UserRole> UserRoles { get; init; } = [];
}
