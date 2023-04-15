using Newtonsoft.Json;

namespace Telemetry_Receiver.Features.V1.WeatherForecast.Dto
{
    [JsonObject]
    public class AddressApiModel
    {
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "street_name")]
        public string StreetName { get; set; }

        [JsonProperty(PropertyName = "street_address")]
        public string StreetAddress { get; set; }
    }
}
