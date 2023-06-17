using Xunit;

namespace Telemetry_Receiver.FunctionalTests.Seedwork
{
    [CollectionDefinition(nameof(WeatherForecastServerCollection))]
    public class WeatherForecastServerCollection
        : ICollectionFixture<WeatherForecastServerFixture>
    {
    }
}
