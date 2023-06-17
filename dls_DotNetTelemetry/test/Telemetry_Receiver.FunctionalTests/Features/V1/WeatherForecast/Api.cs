using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry_Receiver.FunctionalTests.Features.V1.WeatherForecast
{
    internal static class Api
    {
        internal static class V1
        {
            const string VERSION = "1.0";

            internal static class Configuration
            {
                internal static class Get
                {
                    internal static string Configuration() => $"api/WeatherForecast/health?api-version={VERSION}";
                }
            }
        }

    }
}
