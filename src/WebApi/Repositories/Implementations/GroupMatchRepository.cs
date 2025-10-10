using WebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.Context.Implementations;

namespace WebApi.Repositories.Implementations;

public class GroupMatchRepository : BaseRepository<GroupMatch>, IGroupMatchRepository
{
    public GroupMatchRepository(AppDbContext context) : base(context) { }
}
