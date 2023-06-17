using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Telemetry_Receiver.Extensions;
using Telemetry_Receiver.Features.V1.WeatherForecast;

namespace Telemetry_Receiver.FunctionalTests.Seedwork
{
    public class TestStartup
    {
        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        public TestStartup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.Configuration = configuration;
            this.Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddObservability(new TestLoggingBuilder(services), Environment, Configuration)
                .AddApplicationOptions(Configuration)
                .AddApplicationServices(Configuration)
                .AddVersioning()
                .AddMvc()
                .AddApplicationPart(typeof(WeatherForecastController).Assembly);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


        private class TestLoggingBuilder
            : ILoggingBuilder
        {
            private readonly IServiceCollection _services;

            public TestLoggingBuilder(IServiceCollection services)
            {
                _services = services ?? throw new ArgumentNullException(nameof(services));
            }

            public IServiceCollection Services => _services;
        }
    }
}
