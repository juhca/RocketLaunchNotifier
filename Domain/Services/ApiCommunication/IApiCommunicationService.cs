using Domain.Models.RocketLaunch;

namespace Domain.Services.ApiCommunication;

public interface IApiCommunicationService
{
    Task<LaunchApiResponse?> FetchLaunchDataAsync(
        DateTime? simulateStartTime,
        string lastUpdatedQuery
    );
}
