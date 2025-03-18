using Domain.Services.Email;
using Infrastructure.Repositories.EmailRepository;
using Microsoft.AspNetCore.Mvc;
using RocketLauncherNotifier.Controllers.DTO;

namespace RocketLauncherNotifier.Controllers;

/// <summary>
/// API endpoints for managing email subscriptions.
/// </summary>
[ApiController]
[Route("api/email")]
public class EmailController : ControllerBase
{
    private readonly IEmailRepository _emailRepository;
    private readonly EmailService _emailService;

    public EmailController(IEmailRepository emailRepository, EmailService emailService)
    {
        _emailRepository = emailRepository;
        _emailService = emailService;
    }

    /// <summary>
    /// Retrieves all email subscriptions.
    /// </summary>
    /// <returns>A list of all email addresses.</returns>
    /// <response code="200">Returns the list of email addresses.</response>
    /// <response code="404">If no emails are found.</response>
    [HttpGet("get-all", Name = "GetEmails")]
    [ProducesResponseType(typeof(AllEmailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<AllEmailsDto> GetEmails()
    {
        var allEmails = _emailService.GetAllEmails();
        if (allEmails.Count() == 0)
        {
            return NotFound("No emails found");
        }

        return Ok(new AllEmailsDto() { Emails = allEmails });
    }

    /// <summary>
    /// Retrieves a specific email subscription by its ID.
    /// </summary>
    /// <param name="emailId">The ID of the email to retrieve.</param>
    /// <returns>The email address information.</returns>
    /// <response code="200">Returns the email address information.</response>
    /// <response code="400">If the provided ID is invalid or no email is found with that ID.</response>
    [HttpGet("get-by-id", Name = "GetEmailById")]
    [ProducesResponseType(typeof(EmailEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<EmailEntryDto> GetEmailById(int emailId)
    {
        if (emailId <= 0)
        {
            return BadRequest("Invalid Id");
        }

        var entry = _emailService.GetEmailById(emailId);
        if (entry == null)
        {
            return BadRequest("No email found");
        }

        return Ok(new EmailEntryDto() { EmailEntity = entry });
    }

    /// <summary>
    /// Saves a new email subscription.
    /// </summary>
    /// <param name="email">The email address to save.</param>
    /// <returns>A success message if the email was saved.</returns>
    /// <response code="200">Returns a success message.</response>
    /// <response code="400">If the email is empty, has an invalid format, or could not be saved.</response>
    [HttpPost("save-email", Name = "SaveEmail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SaveEmail([FromBody] string email)
    {
        if (email == string.Empty)
        {
            return BadRequest("Email is required.");
        }

        if (!EmailService.IsValidEmail(email))
        {
            return BadRequest("Invalid email format.");
        }

        try
        {
            if (await _emailService.SaveEmail(email))
            {
                return Ok("Email saved successfully.");
            }

            return BadRequest("Email could not be saved.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing email subscription.
    /// </summary>
    /// <param name="email">The new email address.</param>
    /// <param name="emailId">The ID of the email to update.</param>
    /// <returns>The updated email address.</returns>
    /// <response code="200">Returns the updated email address.</response>
    /// <response code="400">If the email is empty, has an invalid format, the ID is invalid, or the email, for some reason could not be updated.</response>
    [HttpPut("update-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult UpdateEmail(string email, int emailId)
    {
        if (email == string.Empty)
        {
            return BadRequest("Email is required.");
        }

        if (!EmailService.IsValidEmail(email))
        {
            return BadRequest("Invalid email format.");
        }

        if (emailId < 1)
        {
            return BadRequest("Invalid email ID.");
        }

        try
        {
            var updatedEmail = _emailService.UpdateEmail(emailId, email);
            if (updatedEmail == null)
            {
                return BadRequest("Email could not be updated.");
            }

            return Ok($"Updated email: {updatedEmail.Email}");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deletes an email subscription by its ID.
    /// </summary>
    /// <param name="emailId">The ID of the email to delete.</param>
    /// <returns>A success message if the email was deleted.</returns>
    /// <response code="200">Returns a success message.</response>
    /// <response code="400">If the provided ID is invalid or the email could not be deleted.</response>
    [HttpDelete("delete-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult DeleteEmailEntry(int emailId)
    {
        if (emailId < 1)
        {
            return BadRequest("Invalid email ID.");
        }

        try
        {
            if (_emailService.DeleteEmail(emailId))
            {
                return Ok("Email deleted successfully.");
            }

            return BadRequest("Email could not be deleted.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
