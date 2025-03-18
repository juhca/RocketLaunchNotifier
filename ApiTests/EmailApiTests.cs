using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RocketLauncherNotifier;
using RocketLauncherNotifier.Controllers.DTO;

namespace ApiTests;

[Trait("Category", "ApiTests")]
public class EmailControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public EmailControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllEmails_ShouldReturnListOfEmails()
    {
        var response = await _client.GetAsync("/api/email/get-all");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var emails = await response.Content.ReadFromJsonAsync<AllEmailsDto>();
        emails.Should().NotBeNull();
        emails.Emails.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetEmailById_ExistingId_ShouldReturnEmail()
    {
        var response = await _client.GetAsync("/api/email/get-by-id?emailId=2");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var emailEntry = await response.Content.ReadFromJsonAsync<EmailEntryDto>();
        emailEntry.Should().NotBeNull();
    }

    [Fact]
    public async Task GetEmailById_NonExistentId_ShouldReturnBadRequest()
    {
        var response = await _client.GetAsync("/api/email/get-by-id?emailId=999");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SaveEmail_ValidEmail_ShouldReturnOk()
    {
        var response = await _client.PostAsJsonAsync("/api/email/save-email", "test@example.com");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SaveEmail_InvalidEmail_ShouldReturnBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/email/save-email", "invalid-email");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteEmail_ExistingEmail_ShouldReturnOk()
    {
        var response = await _client.DeleteAsync("/api/email/delete-email?emailId=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteEmail_NonExistentEmail_ShouldReturnOk()
    {
        var response = await _client.DeleteAsync("/api/email/delete-email?emailId=999");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetEmailById_NonExistentId_ShouldReturnNotFoundWithMessage()
    {
        var response = await _client.GetAsync("/api/email/get-by-id?emailId=999");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("No email found");
    }

    [Fact]
    public async Task GetEmailById_InvalidId_ShouldReturnBadRequestWithMessage()
    {
        var response = await _client.GetAsync("/api/email/get-by-id?emailId=0");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Invalid Id");
    }

    [Fact]
    public async Task SaveEmail_EmptyEmail_ShouldReturnBadRequestWithMessage()
    {
        var response = await _client.PostAsJsonAsync("/api/email/save-email", "");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Email is required.");
    }
}
