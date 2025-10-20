namespace WebApi.Models;

public class Role : TrackableEntity
{
    public string Name { get; init; } = null!;
    // TODO: fazer listas readonly
    public List<UserRole> UserRoles { get; init; } = [];

    private Role() { }
}
