using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.GroupAggregate.Entities;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Groups;

public class GroupPreferenceRepository
    : BaseRepository<GroupPreference>, IGroupPreferenceRepository
{
    public GroupPreferenceRepository(AppDbContext context) : base(context) { }

    public async Task<GroupPreference?> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken)
    {
        var groupPreference = await _dbSet
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.GroupId == groupId, cancellationToken);

        return groupPreference is not null ? new GroupPreference(groupPreference.LikesShopping, groupPreference.LikesGastronomy,
            groupPreference.Culture, groupPreference.Entertainment, groupPreference.PlaceTypes) : null;
    }
}
