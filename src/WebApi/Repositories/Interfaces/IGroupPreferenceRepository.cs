using WebApi.Models.Aggregates;

namespace WebApi.Repositories.Interfaces;

public interface IGroupPreferenceRepository : IBaseRepository<GroupPreference>
{
    Task<GroupPreference?> GetByGroupIdAsync(Guid groupId);
}
