using System.Collections.Generic;
using Infrastructure.Entities;

namespace Infrastructure.Repositories.RocketLaunch;

public interface IRocketLaunchRepository
{
    public IEnumerable<RocketLaunchEntity> GetAllRocketLaunches();
    public IEnumerable<RocketLaunchEntity> GetUpcomingRocketLaunches();

    public RocketLaunchEntity? GetRocketLaunchById(string launchId);

    void SaveRocketLaunch(RocketLaunchEntity entity);

    void UpdateRocketLaunch(string launchId, RocketLaunchEntity entity);

    void DeleteRocketLaunch(string launchId);
}
