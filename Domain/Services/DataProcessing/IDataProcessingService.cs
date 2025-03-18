using Domain.Models.RocketLaunch;

namespace Domain.Services.DataProcessing;

public interface IDataProcessingService
{
    (
        List<SimplifiedLaunch> newLaunches,
        List<(
            SimplifiedLaunch launch,
            DateTime? newLaunchDateTime,
            string newStatus
        )> updatedLaunches
    ) ProcessLaunchData(LaunchApiResponse launchApiData, string lastUpdatedQuery);

    void SaveNewLaunch(SimplifiedLaunch launch);
    void UpdateLaunch(SimplifiedLaunch launch);
}
