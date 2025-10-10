using WebApi.Context.Implementations;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class DestinationRepository : BaseRepository<Destination>, IDestinationRepository
{
    public DestinationRepository(AppDbContext context) : base(context) { }
}
