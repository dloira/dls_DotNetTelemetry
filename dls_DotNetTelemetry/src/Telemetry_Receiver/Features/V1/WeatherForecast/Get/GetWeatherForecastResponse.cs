namespace Telemetry_Receiver.Features.V1.WeatherForecast.Get
{
    public class GetWeatherForecastResponse
    {
        public string City { get; set; } = default!;

        public string Address { get; set; } = default!;

        public DateTime Date { get; set; } = default!;

        public int TemperatureC { get; set; } = default!;

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; } = default!;
    }
}
