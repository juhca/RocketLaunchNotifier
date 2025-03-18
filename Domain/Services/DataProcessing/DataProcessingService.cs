using Domain.Models.RocketLaunch;
using Infrastructure.Entities;
using Infrastructure.Repositories.RocketLaunch;
using Microsoft.Extensions.Logging;

namespace Domain.Services.DataProcessing;

public class DataProcessingService : IDataProcessingService
{
    private readonly ILogger<DataProcessingService> _logger;
    IRocketLaunchRepository _rocketLaunchRepository;

    public DataProcessingService(
        ILogger<DataProcessingService> logger,
        IRocketLaunchRepository rocketLaunchRepository
    )
    {
        _logger = logger;
        _rocketLaunchRepository = rocketLaunchRepository;
    }

    public (
        List<SimplifiedLaunch> newLaunches,
        List<(
            SimplifiedLaunch launch,
            DateTime? newLaunchDateTime,
            string newStatus
        )> updatedLaunches
    ) ProcessLaunchData(LaunchApiResponse launchApiData, string lastUpdatedQuery)
    {
        var newLaunches = new List<SimplifiedLaunch>();
        var updatedLaunches =
            new List<(SimplifiedLaunch launch, DateTime? newLaunchDateTime, string newStatus)>();

        foreach (var launch in launchApiData.Launches)
        {
            // Only at startup when no data
            if (string.IsNullOrEmpty(lastUpdatedQuery))
            {
                SaveNewLaunch(launch);
                newLaunches.Add(launch);
                continue;
            }

            var exists = _rocketLaunchRepository.GetRocketLaunchById(launch.Id);

            if (exists == null)
            {
                SaveNewLaunch(launch);
                newLaunches.Add(launch);
                continue;
            }

            DateTime? newLaunchDateTime = null;
            if (exists.LaunchDate != launch.LaunchDate)
            {
                newLaunchDateTime = launch.LaunchDate;
            }

            string newStatus = string.Empty;
            if (exists.LaunchStatus != launch.Status.Abbrev)
            {
                if (
                    launch.Status.Abbrev == "GO"
                    || launch.Status.Abbrev == "Hold"
                    || launch.Status.Abbrev == "TBC"
                    || launch.Status.Abbrev == "TBD"
                )
                {
                    newStatus = launch.Status.Abbrev;
                    _logger.LogInformation(
                        $"Launch status changed for '{launch.Name}' (ID: {launch.Id}) from '{exists.LaunchStatus}' to '{launch.Status.Abbrev}'"
                    );
                }
            }

            if (newLaunchDateTime == null && newStatus == string.Empty)
            {
                UpdateLaunch(launch);
            }
            else
            {
                UpdateLaunch(launch);
                updatedLaunches.Add((launch, newLaunchDateTime, newStatus));
            }
        }

        return (newLaunches, updatedLaunches);
    }

    public void SaveNewLaunch(SimplifiedLaunch launch)
    {
        try
        {
            _rocketLaunchRepository.SaveRocketLaunch(
                new RocketLaunchEntity
                {
                    RockerLaunchEntryId = launch.Id,
                    Url = launch.Url,
                    Name = launch.Name,
                    LastUpdated = launch.LastUpdated,
                    LaunchDate = launch.LaunchDate,
                    LaunchStatus = launch.Status.Abbrev,
                    LaunchDescription = launch.Status.Description,
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to save launch: {ex.Message}");
            throw;
        }
    }

    public void UpdateLaunch(SimplifiedLaunch launch)
    {
        try
        {
            _rocketLaunchRepository.UpdateRocketLaunch(
                launch.Id,
                new RocketLaunchEntity
                {
                    RockerLaunchEntryId = launch.Id,
                    Url = launch.Url,
                    Name = launch.Name,
                    LastUpdated = launch.LastUpdated,
                    LaunchDate = launch.LaunchDate,
                    LaunchStatus = launch.Status.Abbrev,
                    LaunchDescription = launch.Status.Description,
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to update launch: {ex.Message}");
            throw;
        }
    }
}
