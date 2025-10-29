using WebApi.Models;

namespace WebApi.Repositories.Interfaces;

public interface IDestinationRepository : IBaseRepository<Destination>
{
    Task<(IEnumerable<Destination> data, int hits)> GetNotVotedByUserInGroupAsync(Guid userId, Guid groupId, int pageNumber = 1, int pageSize = 10);
}
