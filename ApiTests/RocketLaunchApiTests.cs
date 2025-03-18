using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Infrastructure.Entities;
using Infrastructure.Repositories.RocketLaunch;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RocketLauncherNotifier;

namespace ApiTests;

[Trait("Category", "ApiTests")]
public class RocketLaunchApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly IRocketLaunchRepository _rocketLaunchRepository;

    public RocketLaunchApiTests(WebApplicationFactory<Program> factory)
    {
        // Override the ConfigureWebHost to register the repository.
        var clientFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing repository registration (if any).
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(IRocketLaunchRepository)
                );
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add your InMemory repository for testing.
                var inMemoryRepo = new InMemoryRocketLaunchRepository(10);
                services.AddSingleton<IRocketLaunchRepository>(inMemoryRepo);
            });
        });

        _rocketLaunchRepository =
            clientFactory.Services.GetRequiredService<IRocketLaunchRepository>();
        _client = clientFactory.CreateClient();
    }

    [Fact(Skip = "Don't test this too much")]
    public async Task GetAllRocketLaunches_ShouldReturnListOfRocketLaunches()
    {
        var response = await _client.GetAsync("/api/rocket-launches/get-all-from-api");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSavedRocketLaunches_ShouldReturnListOfSavedRocketLaunches()
    {
        var response = await _client.GetAsync("/api/rocket-launches/get-saved");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var launches = await response.Content.ReadFromJsonAsync<List<RocketLaunchEntity>>();

        launches.Should().HaveCount(10);
    }

    [Fact]
    public async Task GetUpcomingRocketLaunches_ShouldReturnListOfUpcomingRocketLaunches()
    {
        var response = await _client.GetAsync("/api/rocket-launches/get-upcoming");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var launches = await response.Content.ReadFromJsonAsync<List<RocketLaunchEntity>>();

        launches.Should().HaveCount(5);
    }
}
