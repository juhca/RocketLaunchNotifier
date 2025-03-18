using Infrastructure.Entities;

namespace Infrastructure.Repositories.RocketLaunch;

public class InMemoryRocketLaunchRepository : IRocketLaunchRepository
{
    private readonly Dictionary<string, RocketLaunchEntity> _rocketLaunches;

    public InMemoryRocketLaunchRepository()
    {
        _rocketLaunches = new Dictionary<string, RocketLaunchEntity>();
    }

    // used mostly for tests
    public InMemoryRocketLaunchRepository(int count)
    {
        _rocketLaunches = new Dictionary<string, RocketLaunchEntity>();

        for (var i = 0; i < count; i++)
        {
            // Alternate between past(even) and future(odd) dates
            DateTime launchDate =
                i % 2 == 0 ? DateTime.UtcNow.AddDays(-i - 1) : DateTime.UtcNow.AddDays(i + 1);

            _rocketLaunches[$"rocket_launch_id_{i}"] = new()
            {
                RockerLaunchEntryId = $"rocket_launch_id_{i}",
                LastUpdated = DateTime.UtcNow.AddDays(i),
                LaunchDate = launchDate,
                LaunchDescription = $"Launch description {i}",
                LaunchStatus = "Success",
                Name = $"Name-{i}",
                Url = $"www.rocketlaunch.com/{i}",
            };
        }
    }

    public IEnumerable<RocketLaunchEntity> GetAllRocketLaunches()
    {
        return _rocketLaunches.Values;
    }

    public IEnumerable<RocketLaunchEntity> GetUpcomingRocketLaunches()
    {
        return _rocketLaunches.Values.Where(x => x.LaunchDate >= DateTime.Today);
    }

    public RocketLaunchEntity? GetRocketLaunchById(string launchId)
    {
        _rocketLaunches.TryGetValue(launchId, out var rocketLaunch);

        return rocketLaunch;
    }

    public void SaveRocketLaunch(RocketLaunchEntity entity)
    {
        _rocketLaunches[entity.RockerLaunchEntryId] = entity;
    }

    public void UpdateRocketLaunch(string launchId, RocketLaunchEntity entity)
    {
        if (_rocketLaunches.ContainsKey(launchId))
        {
            _rocketLaunches[launchId] = entity;
        }
    }

    public void DeleteRocketLaunch(string launchId)
    {
        _rocketLaunches.Remove(launchId);
    }
}
