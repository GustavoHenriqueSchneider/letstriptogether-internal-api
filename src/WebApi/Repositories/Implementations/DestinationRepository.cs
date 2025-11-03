using Microsoft.EntityFrameworkCore;
using WebApi.Context.Implementations;
using WebApi.Models.Aggregates;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class DestinationRepository : BaseRepository<Destination>, IDestinationRepository
{
    private readonly AppDbContext _context;

    public DestinationRepository(AppDbContext context) : base(context) 
    {
        _context = context;
    }

    public async Task<(IEnumerable<Destination> data, int hits)> GetNotVotedByUserInGroupAsync(
        Guid userId, Guid groupId, int pageNumber = 1, int pageSize = 10)
    {
        // Busca o groupMemberId do usuário no grupo
        var groupMemberIds = await _context.GroupMembers
            .AsNoTracking()
            .Where(gm => gm.GroupId == groupId && gm.UserId == userId)
            .Select(gm => gm.Id)
            .ToListAsync();

        // Busca todos os destinos que NÃO foram votados pelo usuário neste grupo
        var votedDestinationIds = await _context.GroupMemberDestinationVotes
            .AsNoTracking()
            .Where(gmdv => groupMemberIds.Contains(gmdv.GroupMemberId))
            .Select(gmdv => gmdv.DestinationId)
            .Distinct()
            .ToListAsync();

        // Busca os destinos não votados com paginação
        var data = await _dbSet
            .AsNoTracking()
            .Where(d => !votedDestinationIds.Contains(d.Id))
            .OrderByDescending(d => d.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var hits = await _dbSet
            .AsNoTracking()
            .Where(d => !votedDestinationIds.Contains(d.Id))
            .CountAsync();

        return (data, hits);
    }

    public new async Task<(IEnumerable<Destination> data, int hits)> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Include(d => d.Attractions)
            .OrderByDescending(e => e.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var hits = await _dbSet.CountAsync();

        return (data, hits);
    }

    public new async Task<Destination?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(d => d.Attractions)
            .SingleOrDefaultAsync(e => e.Id == id);
    }
}
