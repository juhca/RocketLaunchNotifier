using System.Text.Json;
using Domain.Models.RocketLaunch;
using Domain.Services.ApiCommunication;
using Domain.Services.DataProcessing;
using Infrastructure.Entities;
using Infrastructure.Repositories.ApiCallTrackingRepository;
using Infrastructure.Repositories.RocketLaunch;
using Microsoft.Extensions.Logging;

namespace Domain.Services.RocketLaunch;

public class RocketLaunchService
{
    private readonly ILogger<RocketLaunchService> _logger;
    private readonly IApiCommunicationService _apiService;
    private readonly IDataProcessingService _dataService;
    private readonly IApiCallTrackingRepository _apiCallTrackingRepository;
    private readonly IRocketLaunchRepository _rocketLaunchRepository;

    public RocketLaunchService(
        ILogger<RocketLaunchService> logger,
        IApiCommunicationService apiService,
        IDataProcessingService dataService,
        IApiCallTrackingRepository apiCallTrackingRepository,
        IRocketLaunchRepository rocketLaunchRepository
    )
    {
        _logger = logger;
        _apiService = apiService;
        _dataService = dataService;
        _apiCallTrackingRepository = apiCallTrackingRepository;
        _rocketLaunchRepository = rocketLaunchRepository;
    }

    // simulatedStartTime ~ if set only launches around that date will be retrieved
    public async Task<RocketLaunchOverview> GetLaunchesAsync(DateTime? simulatedStartTime)
    {
        try
        {
            var apiCallStartTime = simulatedStartTime ?? DateTime.UtcNow;

            // checks if any successfull api req was already made, and if so appends last_updated_gte (greater and equal) parameter
            var lastUpdatedQuery = BuildLastUpdatedQuery(simulatedStartTime);

            var launchData = await _apiService.FetchLaunchDataAsync(
                simulatedStartTime,
                lastUpdatedQuery
            );

            if (launchData == null)
            {
                return new RocketLaunchOverview(null, [], [], "No results found");
            }

            var (newLaunches, updatedLaunches) = _dataService.ProcessLaunchData(
                launchData,
                lastUpdatedQuery
            );
            _apiCallTrackingRepository.SetLastSuccessfulCall(apiCallStartTime);

            return new RocketLaunchOverview(launchData, newLaunches, updatedLaunches);
        }
        catch (HttpRequestException ex) // caught exception from _apiService.FetchLaunchDataAsync
        {
            _logger.LogError($"API request failed: {ex.Message}");
            return new RocketLaunchOverview(
                null,
                [],
                [],
                $"API request failed with errors {ex.Message}"
            );
        }
        catch (JsonException ex) // caught exception from _apiService.FetchLaunchDataAsync
        {
            _logger.LogError($"JSON deserialization failed: {ex.Message}");
            return new RocketLaunchOverview(
                null,
                [],
                [],
                $"JSON deserialization failed with errors {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error: {ex.Message}");
            return new RocketLaunchOverview(null, [], [], $"Unexpected error: {ex.Message}");
        }
    }

    public List<RocketLaunchEntity> GetSavedLaunchesAsync()
    {
        var result = _rocketLaunchRepository.GetAllRocketLaunches();

        return result.ToList();
    }

    public List<RocketLaunchEntity> GetUpcomingLaunchesAsync()
    {
        var result = _rocketLaunchRepository.GetUpcomingRocketLaunches();

        return result.ToList();
    }

    private string BuildLastUpdatedQuery(DateTime? dateTime)
    {
        DateTime? lastSuccessfulCall =
            dateTime ?? _apiCallTrackingRepository.GetLastSuccessfulCall();

        if (lastSuccessfulCall == null)
        {
            return string.Empty;
        }

        var formattedDate = lastSuccessfulCall.Value.ToString("yyyy-MM-ddTHH:mm:ss");
        return $"&last_updated__gte={formattedDate}";
    }
}
