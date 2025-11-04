using WebApi.Models.Aggregates;

namespace WebApi.Repositories.Interfaces;

public interface IGroupMatchRepository : IBaseRepository<GroupMatch>
{
    Task<GroupMatch?> GetByIdWithRelationsAsync(Guid groupId, Guid id);
    Task<(IEnumerable<GroupMatch> data, int hits)> GetByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10);
    Task<GroupMatch?> GetByGroupAndDestinationAsync(Guid groupId, Guid destinationId);
    Task<IEnumerable<GroupMatch>> GetAllMatchesByGroupAsync(Guid groupId);
}
