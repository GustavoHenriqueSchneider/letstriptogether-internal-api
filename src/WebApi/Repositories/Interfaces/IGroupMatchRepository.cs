using WebApi.Models;

namespace WebApi.Repositories.Interfaces;

public interface IGroupMatchRepository : IBaseRepository<GroupMatch>
{
    Task<GroupMatch?> GetByIdWithRelationsAsync(Guid id);
    Task<(IEnumerable<GroupMatch> data, int hits)> GetByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10);
}