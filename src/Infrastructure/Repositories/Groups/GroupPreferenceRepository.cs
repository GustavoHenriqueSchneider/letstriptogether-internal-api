using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;

namespace LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;

public class GroupPreferenceRepository
    : BaseRepository<GroupPreference>, IGroupPreferenceRepository
{
    public GroupPreferenceRepository(AppDbContext context) : base(context) { }

    public async Task<GroupPreference?> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken)
    {
        var groupPreference = await _dbSet
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.GroupId == groupId, cancellationToken);

        return groupPreference is not null ? new GroupPreference(groupPreference.LikesCommercial, groupPreference.Food,
            groupPreference.Culture, groupPreference.Entertainment, groupPreference.PlaceTypes) : null;
    }
}
