using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Telemetry_Receiver.FunctionalTests.Seedwork
{
    public class WeatherForecastServerFixture
        : IAsyncLifetime
    {
        public TestServer TestServer { get; private set; } = default!;

        private IHost _host = default!;

        public async Task InitializeAsync()
        {
            await CreateServerHost();
        }

        public Task DisposeAsync()
        {
            _host.Dispose();
            TestServer.Dispose();
            return Task.CompletedTask;
        }

        private async Task CreateServerHost()
        {
            _host = new HostBuilder()
                .UseEnvironment("Development")
                .ConfigureWebHost(builder =>
                {
                    builder
                    .ConfigureServices(services => services.AddSingleton<IServer>(serviceProvider => new TestServer(serviceProvider)))
                    .UseStartup<TestStartup>();
                })
                .ConfigureAppConfiguration((_, configurationBuilder) =>
                {
                    BuildTestconfigurationBuilder(configurationBuilder);
                })
                .Build();

            await _host.StartAsync();

            TestServer = _host.GetTestServer();
        }

        private IConfigurationBuilder BuildTestconfigurationBuilder(IConfigurationBuilder builder)
        {
            return builder.AddJsonFile("appsettings.Testing.json")
                .AddEnvironmentVariables();
        }
    }
}
