using Domain.Models.RocketLaunch;

namespace RocketLauncherNotifier.Controllers.DTO;

public class RocketLaunchDto
{
    public RocketLaunchOverview LaunchData { get; set; }
    public string EmailResponse { get; set; }

    public RocketLaunchDto(RocketLaunchOverview launchData, string emailResponse)
    {
        LaunchData = launchData;
        EmailResponse = emailResponse;
    }
}
