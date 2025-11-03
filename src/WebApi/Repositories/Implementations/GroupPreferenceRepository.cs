using Microsoft.EntityFrameworkCore;
using WebApi.Context.Implementations;
using WebApi.Models.Aggregates;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class GroupPreferenceRepository
    : BaseRepository<GroupPreference>, IGroupPreferenceRepository
{
    public GroupPreferenceRepository(AppDbContext context) : base(context) { }

    public async Task<GroupPreference?> GetByGroupIdAsync(Guid groupId)
    {
        var groupPreference = await _dbSet
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.GroupId == groupId);

        return groupPreference is not null ? new GroupPreference(groupPreference.LikesCommercial, groupPreference.Food,
            groupPreference.Culture, groupPreference.Entertainment, groupPreference.PlaceTypes) : null;
    }
}
