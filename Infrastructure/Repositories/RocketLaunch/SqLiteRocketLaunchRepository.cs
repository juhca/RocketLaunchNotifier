using Infrastructure.Data;
using Infrastructure.Entities;

namespace Infrastructure.Repositories.RocketLaunch;

public class SqLiteRocketLaunchRepository : IRocketLaunchRepository
{
    private readonly RocketLaunchDbContext _context;

    public SqLiteRocketLaunchRepository(RocketLaunchDbContext context)
    {
        _context = context;
    }

    public IEnumerable<RocketLaunchEntity> GetAllRocketLaunches()
    {
        return _context.RocketLaunches.ToList();
    }

    public IEnumerable<RocketLaunchEntity> GetUpcomingRocketLaunches()
    {
        return _context.RocketLaunches.Where(x => x.LaunchDate >= DateTime.Today).ToList();
    }

    public RocketLaunchEntity? GetRocketLaunchById(string launchId)
    {
        return _context.RocketLaunches.Find(launchId);
    }

    public void SaveRocketLaunch(RocketLaunchEntity entity)
    {
        _context.RocketLaunches.Add(entity);
        _context.SaveChanges();
    }

    public void UpdateRocketLaunch(string launchId, RocketLaunchEntity entity)
    {
        var existingLaunch = _context.RocketLaunches.Find(launchId);
        if (existingLaunch != null)
        {
            // Update properties
            existingLaunch.Name = entity.Name;
            existingLaunch.Url = entity.Url;
            existingLaunch.LastUpdated = entity.LastUpdated;
            existingLaunch.LaunchDate = entity.LaunchDate;
            existingLaunch.LaunchStatus = entity.LaunchStatus;
            existingLaunch.LaunchDescription = entity.LaunchDescription;

            _context.SaveChanges();
        }
    }

    public void DeleteRocketLaunch(string launchId)
    {
        var launchToRemove = _context.RocketLaunches.Find(launchId);
        if (launchToRemove != null)
        {
            _context.RocketLaunches.Remove(launchToRemove);
            _context.SaveChanges();
        }
    }
}
