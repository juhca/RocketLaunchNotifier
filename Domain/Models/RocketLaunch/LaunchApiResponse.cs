using System.Text.Json.Serialization;

namespace Domain.Models.RocketLaunch;

public record LaunchApiResponse(
    int Count,
    string Next,
    string Previous,
    [property: JsonPropertyName("results")] SimplifiedLaunch[] Launches
);

public record SimplifiedLaunch(
    string Id,
    string Url,
    string Name,
    [property: JsonPropertyName("last_updated")] DateTime LastUpdated,
    [property: JsonPropertyName("net")] DateTime LaunchDate,
    LaunchStatus Status
);

public record LaunchStatus(string Abbrev, string Description);
