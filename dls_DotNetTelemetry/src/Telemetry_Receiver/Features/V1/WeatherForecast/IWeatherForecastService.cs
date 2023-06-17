using Telemetry_Receiver.Features.V1.WeatherForecast.Get;

namespace Telemetry_Receiver.Features.V1.WeatherForecast
{
    public interface IWeatherForecastService
    {
        Task<IEnumerable<GetWeatherForecastResponse>?> GetWeatherForecastDataAsync();

    }
}
