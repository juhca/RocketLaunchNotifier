using Domain.Models.RocketLaunch;
using Domain.Services.EmailNotification;
using Infrastructure.Entities;
using Infrastructure.Repositories.EmailRepository;
using Infrastructure.Repositories.RocketLaunch;
using Microsoft.Extensions.Logging;

namespace Domain.Services.Email;

public class EmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IEmailRepository _emailRepository;
    private readonly IRocketLaunchRepository _rocketLaunchRepository;
    private readonly EmailNotificationService _emailNotificationService;

    public EmailService(
        ILogger<EmailService> logger,
        IEmailRepository emailRepository,
        IRocketLaunchRepository rocketLaunchRepository,
        EmailNotificationService emailNotificationService
    )
    {
        _logger = logger;
        _emailRepository = emailRepository;
        _rocketLaunchRepository = rocketLaunchRepository;
        _emailNotificationService = emailNotificationService;
    }

    public List<string> GetAllEmails()
    {
        return _emailRepository.GetAllEmails().ToList();
    }

    public EmailEntity? GetEmailById(int id)
    {
        return _emailRepository.GetById(id);
    }

    public async Task<RocketLaunchEmailNotificationOverview> SendEmail(
        List<SimplifiedLaunch> newLaunches,
        List<(
            SimplifiedLaunch launch,
            DateTime? newLaunchDateTime,
            string newStatus
        )> updatedLaunches
    )
    {
        var emailResult = await _emailNotificationService.SendEmail(newLaunches, updatedLaunches);

        if (emailResult.Success)
        {
            return new RocketLaunchEmailNotificationOverview("Success", null);
        }

        _logger.LogError($"Email notification fail: {emailResult?.Exception?.Message}");
        return new RocketLaunchEmailNotificationOverview(
            "Failure",
            emailResult?.Exception?.Message
        );
    }

    public async Task<bool> SaveEmail(string email)
    {
        try
        {
            _emailRepository.SaveEmail(email);

            var upcomingLaunch = _rocketLaunchRepository.GetUpcomingRocketLaunches().ToList();

            if (upcomingLaunch.Count != 0)
            {
                List<SimplifiedLaunch> simplifiedLaunches = [];
                simplifiedLaunches.AddRange(
                    upcomingLaunch.Select(launch => new SimplifiedLaunch(
                        launch.RockerLaunchEntryId,
                        launch.Url,
                        launch.Name,
                        launch.LastUpdated,
                        launch.LaunchDate,
                        new LaunchStatus(launch.LaunchStatus, launch.LaunchDescription)
                    ))
                );
                await _emailNotificationService.SendEmail(simplifiedLaunches, [], email);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public EmailEntity? UpdateEmail(int emailId, string email)
    {
        return _emailRepository.UpdateEmail(emailId, email);
    }

    public bool DeleteEmail(int emailId)
    {
        try
        {
            _emailRepository.DeleteEmail(emailId);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidEmail(string email)
    {
        try
        {
            var mail = new System.Net.Mail.MailAddress(email);
            return email.Equals(mail.Address);
        }
        catch
        {
            return false;
        }
    }
}
