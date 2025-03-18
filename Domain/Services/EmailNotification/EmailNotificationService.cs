using System.Text;
using Domain.Models.Email;
using Domain.Models.RocketLaunch;
using Infrastructure.Repositories.EmailRepository;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Domain.Services.EmailNotification;

public class EmailNotificationService
{
    private readonly EmailConfig _config;
    private readonly IEmailRepository _emailRepository;
    private readonly ILogger<EmailNotificationService> _logger;

    public EmailNotificationService(
        EmailConfig config,
        IEmailRepository emailRepository,
        ILogger<EmailNotificationService> logger
    )
    {
        ArgumentNullException.ThrowIfNull(config);

        if (
            string.IsNullOrWhiteSpace(config.SmtpServer)
            || string.IsNullOrWhiteSpace(config.SmtpUsername)
            || string.IsNullOrWhiteSpace(config.SmtpPassword)
        )
        {
            throw new InvalidOperationException("SMTP configuration is incomplete.");
        }

        _config = config;
        _emailRepository = emailRepository;
        _logger = logger;
    }

    public async Task<EmailResult> SendEmail(
        List<SimplifiedLaunch> newLaunches,
        List<(
            SimplifiedLaunch launch,
            DateTime? newLaunchDateTime,
            string newStatus
        )> updatedLaunches,
        string email = ""
    )
    {
        try
        {
            var html = GenerateLaunchSummary(newLaunches, updatedLaunches);
            await SendLaunchNotificationAsync(html, email);

            return new EmailResult(true, "Notification email sent successfully.");
        }
        catch (SmtpCommandException ex)
        {
            return new EmailResult(
                false,
                $"SMTP Command Error: {ex.Message} (StatusCode: {ex.StatusCode})",
                ex
            );
        }
        catch (SmtpProtocolException ex)
        {
            return new EmailResult(false, $"SMTP Protocol Error: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            return new EmailResult(false, $"General Error: {ex.Message}", ex);
        }
    }

    private async Task SendLaunchNotificationAsync(string html, string email = "")
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Launch Notifications", _config.FromAddress));

        if (!string.IsNullOrWhiteSpace(email))
        {
            message.To.Add(new MailboxAddress("", email));
        }
        else
        {
            foreach (var recipient in _emailRepository.GetAllEmails())
            {
                message.To.Add(new MailboxAddress("", recipient));
            }
        }

        message.Subject = $"Launch Notifications ({DateTime.Today.ToShortDateString()})";
        message.Body = new TextPart("html") { Text = html };

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(
                _config.SmtpServer,
                _config.SmtpPort,
                SecureSocketOptions.SslOnConnect
            );
            await client.AuthenticateAsync(_config.SmtpUsername, _config.SmtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation($"Notification email sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending email: {ex.Message}");
            if (ex.InnerException != null)
            {
                _logger.LogError($"Inner exception: {ex.InnerException.Message}");
            }
        }
    }

    // todo:remove sus stuff
    private string GenerateLaunchSummary(
        List<SimplifiedLaunch> newLaunches,
        List<(
            SimplifiedLaunch launch,
            DateTime? newLaunchDateTime,
            string newStatus
        )> updatedLaunches
    )
    {
        if (newLaunches.Count == 0 && updatedLaunches.Count == 0)
        {
            return string.Empty;
        }

        var htmlSummary = new StringBuilder();

        htmlSummary.AppendLine("<html><body>");
        htmlSummary.AppendLine("<style>");
        htmlSummary.AppendLine("body { font-family: Arial, sans-serif; }");
        htmlSummary.AppendLine("h1, h2 { color: #333; }");
        htmlSummary.AppendLine("h2 { margin-top: 20px; }");
        htmlSummary.AppendLine("ul { list-style-type: none; padding-left: 0; }");
        htmlSummary.AppendLine(
            "li { margin-bottom: 10px; padding: 10px; border: 1px solid #eee; border-radius: 5px; }"
        );
        htmlSummary.AppendLine(".launch-name { font-weight: bold; }");
        htmlSummary.AppendLine(".change { margin-left: 20px; font-style: italic; color: #666; }");
        htmlSummary.AppendLine("</style>");

        if (newLaunches.Count != 0)
        {
            htmlSummary.AppendLine("<h1>New Rocket Launches</h1>");
            htmlSummary.AppendLine("<ul>");
            foreach (var launch in newLaunches)
            {
                htmlSummary.AppendLine("<li>");
                htmlSummary.AppendLine(
                    $"<span class='launch-name'>{launch.Name}</span> (ID: {launch.Id})"
                );
                htmlSummary.AppendLine(
                    $"<p>Launch Date: {launch.LaunchDate:yyyy-MM-dd HH:mm UTC}</p>"
                );
                htmlSummary.AppendLine("</li>");
            }
            htmlSummary.AppendLine("</ul>");
        }

        if (updatedLaunches.Count != 0)
        {
            htmlSummary.AppendLine("<h2>Updated Rocket Launches</h2>");
            htmlSummary.AppendLine("<ul>");
            foreach (var (launch, newLaunchDateTime, newStatus) in updatedLaunches)
            {
                htmlSummary.AppendLine("<li>");
                htmlSummary.AppendLine(
                    $"<span class='launch-name'>{launch.Name}</span> (ID: {launch.Id})"
                );
                if (newLaunchDateTime.HasValue)
                {
                    htmlSummary.AppendLine(
                        $"<p class='change'>Launch Date changed to: {newLaunchDateTime:yyyy-MM-dd HH:mm UTC}</p>"
                    );
                }
                if (!string.IsNullOrEmpty(newStatus))
                {
                    htmlSummary.AppendLine($"<p class='change'>Status changed to: {newStatus}</p>");
                }
                htmlSummary.AppendLine("</li>");
            }
            htmlSummary.AppendLine("</ul>");
        }

        htmlSummary.AppendLine("</body></html>");

        return htmlSummary.ToString();
    }
}
