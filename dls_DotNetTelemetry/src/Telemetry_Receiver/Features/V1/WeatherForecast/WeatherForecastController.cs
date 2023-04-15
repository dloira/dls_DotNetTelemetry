using Telemetry_Receiver.Features.V1.WeatherForecast.Get;
using Telemetry_Receiver.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Telemetry_Receiver.Diagnostics;

namespace Telemetry_Receiver.Features.V1.WeatherForecast
{

    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class WeatherForecastController 
        : ControllerBase
    {
        private readonly IOptionsMonitor<TelemetryReceiverOptions> _options;
        private readonly WeatherForecastService _weatherForecastService;
        public WeatherForecastController(WeatherForecastService weatherForecastService, IOptionsMonitor<TelemetryReceiverOptions> options)
        {
            _weatherForecastService = weatherForecastService ?? throw new ArgumentNullException(nameof(weatherForecastService));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetWeatherForecastResponse), 200)]
        [ProducesResponseType(400)]
        public IActionResult Get()
        {
            var response = Task.FromResult(_weatherForecastService.GetWeatherForecastDataAsync());

            return Ok(response);
        }
    }
}
