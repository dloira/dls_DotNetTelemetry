using Telemetry_Receiver.Features.V1.WeatherForecast.Get;
using System;
using Telemetry_Receiver.Diagnostics;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net.Http.Headers;
using System.Net;
using System.Net.Http;
using Telemetry_Receiver.Features.V1.WeatherForecast.Dto;
using Newtonsoft.Json;

namespace Telemetry_Receiver.Features.V1.WeatherForecast
{
    public class WeatherForecastService
    {
        private HttpClient _httpClient;

        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly TelemetryReceiverDiagnostics _diagnostics;

        public WeatherForecastService(TelemetryReceiverDiagnostics diagnostics)
        {
            _diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
            _httpClient = GetAddressWebApiClient();
        }

        internal async Task<IEnumerable<GetWeatherForecastResponse>?> GetWeatherForecastDataAsync()
        {
            _diagnostics.EventReceived();

            // Calling a public API to retrieve random data
            var response = await _httpClient.GetAsync("api/v2/addresses");
            var responseContent = JsonConvert.DeserializeObject<AddressApiModel>(await response.Content.ReadAsStringAsync());


            return Enumerable.Range(1, 5).Select(index => new GetWeatherForecastResponse
            {
                City = responseContent.City,
                Address = responseContent.StreetName + " " + responseContent.StreetAddress,
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        private HttpClient GetAddressWebApiClient()
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri("https://random-data-api.com/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = TimeSpan.FromSeconds(10);

            return client;
        }
    }
}
