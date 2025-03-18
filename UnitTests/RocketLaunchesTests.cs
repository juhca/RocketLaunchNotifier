using Domain.Services.ApiCommunication;
using Domain.Services.DataProcessing;
using Domain.Services.RocketLaunch;
using FluentAssertions;
using Infrastructure.Repositories.ApiCallTrackingRepository;
using Infrastructure.Repositories.RocketLaunch;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests;

[Trait("Category", "UnitTests")]
public class RocketLaunchesTests
{
    private readonly RocketLaunchService _rocketLaunchService;

    public RocketLaunchesTests()
    {
        var loggerMock = new Mock<ILogger<RocketLaunchService>>().Object;
        var rocketLaunchRepository = new InMemoryRocketLaunchRepository(10);
        var apiService = new Mock<IApiCommunicationService>().Object;
        var dataService = new Mock<IDataProcessingService>().Object;
        var apiCallTrackingRepository = new InMemoryApiCallTrackingRepository();
        _rocketLaunchService = new RocketLaunchService(
            loggerMock,
            apiService,
            dataService,
            apiCallTrackingRepository,
            rocketLaunchRepository
        );
    }

    [Fact]
    public void GetSavedLaunches_ShouldReturnListOfLaunches()
    {
        var result = _rocketLaunchService.GetSavedLaunchesAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(10);
    }

    [Fact]
    public void GetUpcomingLaunches_ShouldReturnListOfLaunches()
    {
        var result = _rocketLaunchService.GetUpcomingLaunchesAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(5);
    }
}
