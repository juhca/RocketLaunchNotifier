using Domain.Services.Email;
using Domain.Services.RocketLaunch;
using Microsoft.AspNetCore.Mvc;
using RocketLauncherNotifier.Controllers.DTO;

namespace RocketLauncherNotifier.Controllers;

/// <summary>
/// API endpoints for retrieving information about rocket launches.
/// </summary>
[ApiController]
[Route("api/rocket-launches")]
public class RocketLaunchesController : ControllerBase
{
    private readonly RocketLaunchService _rocketLaunchService;
    private readonly EmailService _emailService;

    public RocketLaunchesController(
        RocketLaunchService rocketLaunchService,
        EmailService emailService
    )
    {
        _rocketLaunchService = rocketLaunchService;
        _emailService = emailService;
    }

    /// <summary>
    /// Retrieves rocket launch data from an external API, optionally filtered by date.
    /// </summary>
    /// <param name="dateTime">Optional: The date to hardcode last successfull api request (yyyy-MM-dd format), so it only retreived after that date.</param>
    /// <returns>Information about new and updated rocket launches, and the status of email notifications.</returns>
    /// <response code="200">Returns the launch data and email notification status.</response>
    /// <response code="400">If there is an error fetching or processing launch data, or sending emails.</response>
    /// <response code="404">If no launches are found based on the criteria.</response>
    [HttpGet("get-all-from-api", Name = "GetRocketLaunchesFromApi")]
    public async Task<ActionResult<RocketLaunchDto>> GetRocketLaunches(DateTime? dateTime)
    {
        try
        {
            var launchData = await _rocketLaunchService.GetLaunchesAsync(dateTime);
            if (launchData.Response == null)
            {
                return NotFound("Launches not found");
            }

            if (launchData.NewLaunches.Count > 0 || launchData.UpdatedLaunches.Count > 0)
            {
                var emailData = await _emailService.SendEmail(
                    launchData.NewLaunches,
                    launchData.UpdatedLaunches
                );

                if (emailData.EmailError != null)
                {
                    return BadRequest($"Error while sending email: {emailData.EmailError}");
                }

                return Ok(new RocketLaunchDto(launchData, emailData.EmailResponse));
            }

            return Ok(new RocketLaunchDto(launchData, "Emails not sent"));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves a list of saved rocket launch entities.
    /// </summary>
    /// <returns>A list of saved rocket launch entities.</returns>
    /// <response code="200">Returns the list of saved rocket launch entities.</response>
    /// <response code="400">If there is an error retrieving the saved launches.</response>
    [HttpGet("get-saved", Name = "GetSavedRocketLaunches")]
    [ProducesResponseType(
        typeof(IEnumerable<Infrastructure.Entities.RocketLaunchEntity>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetSavedRocketLaunches()
    {
        try
        {
            var launchData = _rocketLaunchService.GetSavedLaunchesAsync();

            return Ok(launchData);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves a list of upcoming rocket launch entities.
    /// </summary>
    /// <returns>A list of upcoming rocket launch entities.</returns>
    /// <response code="200">Returns the list of upcoming rocket launch entities.</response>
    /// <response code="400">If there is an error retrieving the upcoming launches.</response>
    /// <response code="404">If no upcoming launches are found.</response>
    [HttpGet("get-upcoming", Name = "GetUpcomingRocketLaunches")]
    [ProducesResponseType(
        typeof(IEnumerable<Infrastructure.Entities.RocketLaunchEntity>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUpcomingRocketLaunches()
    {
        try
        {
            var launchData = _rocketLaunchService.GetUpcomingLaunchesAsync();

            if (launchData.Count == 0)
            {
                return NotFound("No upcoming launches.");
            }

            return Ok(launchData);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
