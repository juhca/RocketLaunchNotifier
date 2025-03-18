using Domain.Services.Email;
using Domain.Services.RocketLaunch;
using Microsoft.Extensions.DependencyInjection;

namespace RocketLauncherNotifier.BackgroundServices;

public class RocketLaunchBackgroundService : BackgroundService
{
    private readonly ILogger<RocketLaunchBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RocketLaunchBackgroundService(
        ILogger<RocketLaunchBackgroundService> logger,
        IServiceScopeFactory serviceScopeFactory
    )
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Rocket Launch Background Service is starting.");

        // Run every hour
        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation("Fetching rocket launches from background service...");

                // Create a scope for each operation
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var rocketLaunchService =
                        scope.ServiceProvider.GetRequiredService<RocketLaunchService>();
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                    // Use the scoped services
                    var launchData = await rocketLaunchService.GetLaunchesAsync(null);

                    if (launchData.Response == null)
                    {
                        continue;
                    }

                    _logger.LogInformation(
                        $"Background service found {launchData.Response?.Count} launches"
                    );

                    if (launchData.NewLaunches.Count > 0 || launchData.UpdatedLaunches.Count > 0)
                    {
                        var emailData = await emailService.SendEmail(
                            launchData.NewLaunches,
                            launchData.UpdatedLaunches
                        );

                        if (emailData.EmailError != null)
                        {
                            _logger.LogInformation(
                                $"Error while sending email: {emailData.EmailError}"
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while fetching rocket launches in background service"
                );
            }
        }
    }
}
