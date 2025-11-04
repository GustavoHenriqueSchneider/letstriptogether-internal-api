using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace LetsTripTogether.InternalApi.Infrastructure.Persistence.Repositories.Groups;

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
