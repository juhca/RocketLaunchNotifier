using System.Text.Json;
using Domain.Models.RocketLaunch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Domain.Services.ApiCommunication;

public class ApiCommunicationService : IApiCommunicationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiCommunicationService> _logger;
    private readonly IConfiguration _configuration;

    public ApiCommunicationService(
        IHttpClientFactory httpClientFactory,
        ILogger<ApiCommunicationService> logger,
        IConfiguration configuration
    )
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<LaunchApiResponse?> FetchLaunchDataAsync(
        DateTime? simulateStartTime,
        string lastUpdatedQuery
    )
    {
        try
        {
            DateTime startDate = simulateStartTime ?? DateTime.UtcNow;

            string dateFrom = startDate.ToString("yyyy-MM-dd");
            string dateTo = startDate.AddDays(7).ToString("yyyy-MM-dd");

            string baseUrl = _configuration.GetSection("SpaceDevsApi")["BaseUrl"];

            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentException("BaseUrl not set");
            }

            string url =
                $"{baseUrl}?mode=list&net__gte={dateFrom}&net__lte={dateTo}&ordering=-last_updated{lastUpdatedQuery}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<LaunchApiResponse>(content, options);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching launch data: {ex.Message}");
            throw;
        }
    }
}
