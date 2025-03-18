namespace Domain.Models.RocketLaunch;

public record RocketLaunchOverview(
    LaunchApiResponse? Response,
    List<SimplifiedLaunch> NewLaunches,
    List<(SimplifiedLaunch launch, DateTime? newLaunchDateTime, string newStatus)> UpdatedLaunches,
    string ExceptionMessage = ""
);

public record RocketLaunchEmailNotificationOverview(string EmailResponse, string? EmailError);
