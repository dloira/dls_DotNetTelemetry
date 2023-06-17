using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Telemetry_Receiver.Features.V1.WeatherForecast;
using Telemetry_Receiver.FunctionalTests.Seedwork;
using Xunit;

namespace Telemetry_Receiver.FunctionalTests.Features.V1.WeatherForecast
{
    [Collection(nameof(WeatherForecastServerCollection))]
    public class WeatherForecastControllerTests
    {
        private readonly WeatherForecastServerFixture _fixture;

        public WeatherForecastControllerTests(WeatherForecastServerFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task health_should_work()
        {
            var response = await _fixture.TestServer.CreateClient()
                .GetAsync(Api.V1.Configuration.Get.Configuration());

            response.Should()
                .NotBeNull();

            response.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);
        }
    }
}
