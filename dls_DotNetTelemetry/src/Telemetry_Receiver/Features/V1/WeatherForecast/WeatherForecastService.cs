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
using Telemetry_Receiver.Infraestructure.QueryReader;
using Microsoft.Data.SqlClient;
using Azure.Core;
using Dapper;

namespace Telemetry_Receiver.Features.V1.WeatherForecast
{
    public class WeatherForecastService
    {
        private readonly SqlConnection _connection;
        private readonly IQueryProviderService _queryProvider;

        private HttpClient _httpClient;

        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly TelemetryReceiverDiagnostics _diagnostics;

        public WeatherForecastService(SqlConnection connection, IQueryProviderService queryProvider, TelemetryReceiverDiagnostics diagnostics)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _queryProvider = queryProvider ?? throw new ArgumentNullException(nameof(queryProvider));
            _diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
            _httpClient = GetAddressWebApiClient();
        }

        internal async Task<IEnumerable<GetWeatherForecastResponse>?> GetWeatherForecastDataAsync()
        {
            _diagnostics.EventReceived();

            // Calling a public API to retrieve random data
            var response = await _httpClient.GetAsync("api/v2/addresses");
            var responseContent = JsonConvert.DeserializeObject<AddressApiModel>(await response.Content.ReadAsStringAsync());

            // Connecting to database to retrieve random data
            try
            {
                var weatherForecasts = _connection.Query<GetWeatherForecastResponse>(_queryProvider.GetQuery(QueryNames.GET_WEATHERFORECAST)).ToList();
                _ = weatherForecasts.Count;
            }
            catch (Exception exception)
            {
                _diagnostics.ErrorGettingWeatherForecast(exception);
            }
            finally
            {
                _connection.Close();
            }


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
