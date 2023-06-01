using Telemetry_Receiver.Diagnostics;
using Telemetry_Receiver.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;

using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Serilog;

using Telemetry_Receiver.Features.V1.WeatherForecast;
using Telemetry_Receiver.Infraestructure.QueryReader;
using Microsoft.Data.SqlClient;

namespace Telemetry_Receiver.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var configurationOptions = configuration.GetSection(nameof(TelemetryReceiverOptions))
                .Get<TelemetryReceiverOptions>();

            return services
                .AddTransient(_ => new SqlConnection(configurationOptions.Database.ConnectionString))
                .AddMemoryCache()
                .AddTransient<WeatherForecastService>()
                .AddSingleton<IQueryProviderService, XmlQueryProviderService>();
        }

        public static IServiceCollection AddVersioning(this IServiceCollection services)
        {
            return services
                .AddVersionedApiExplorer()
                .AddApiVersioning(setup =>
                {
                    setup.AssumeDefaultVersionWhenUnspecified = true;
                    setup.DefaultApiVersion = new ApiVersion(1, 0);
                    setup.ApiVersionReader = ApiVersionReader.Combine(
                        new HeaderApiVersionReader()
                        {
                            HeaderNames = { "x-api-version" }
                        },
                        new QueryStringApiVersionReader()
                        {
                            ParameterNames = { "api-version" }
                        });
                });
        }

        public static IServiceCollection AddOpenApi(this IServiceCollection services)
        {
            return services.AddSwaggerGen(options =>
            {
                // get the api version description provider
                var apiDescriptionProvider = services.BuildServiceProvider()
                    .GetRequiredService<IApiVersionDescriptionProvider>();

                // for each registered version create a new swagger document
                foreach (var apiVersionDescription in apiDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(
                        apiVersionDescription.GroupName,
                        new OpenApiInfo
                        {
                            Version = apiVersionDescription.ApiVersion.ToString(),
                            Title = $"Swagger {apiVersionDescription.ApiVersion.ToString()}",
                            TermsOfService = default,
                            Description = "A collection of api's for testing telemetry",
                            License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") },
                        }
                    );
                }
            });
        }

        public static IServiceCollection AddApplicationOptions(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<TelemetryReceiverOptions>(configuration.GetSection(nameof(TelemetryReceiverOptions)));
        }

        public static IServiceCollection AddObservability(this IServiceCollection services, ILoggingBuilder loggingBuilder, IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            Action<ResourceBuilder> configurerResource = (resourceBuilder) =>
            {
                resourceBuilder.AddService(
                      serviceName: nameof(TelemetryReceiver),
                      serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown",
                      serviceInstanceId: Environment.MachineName);

                resourceBuilder.AddAttributes(new[]
                {
                    new KeyValuePair<string,object>(TelemetryReceiverConstants.COUNTER_TAG_MACHINE_NAME, Environment.MachineName),
                    new KeyValuePair<string,object>(TelemetryReceiverConstants.COUNTER_TAG_ENVIRONMENT_NAME, hostingEnvironment.EnvironmentName),
                });
            };

            // add diagnostic class
            services
                .AddSingleton<TelemetryReceiverLogging>()
                .AddSingleton<TelemetryReceiverDiagnostics>();

            // add instrumentaiton and metrics
            services.AddOpenTelemetry()
                .ConfigureResource(configurerResource)
                .WithTracing(tracingBuilder =>
                {
                    tracingBuilder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation(options =>
                        {
                            options.RecordException = hostingEnvironment.IsDevelopment();
                        })
                        //.AddSqlClientInstrumentation(options =>
                        //{
                        //    options.RecordException = hostingEnvironment.IsDevelopment();
                        //    options.SetDbStatementForText = hostingEnvironment.IsDevelopment();
                        //})
                        .AddSource(nameof(TelemetryReceiver))
                        .AddJaegerExporter();

                    tracingBuilder.ConfigureServices(setup =>
                    {
                        setup.Configure<JaegerExporterOptions>(configuration.GetSection("Jaeger"));
                    });
                })
                .WithMetrics(metricsBuilder =>
                {
                    metricsBuilder
                        .AddMeter(nameof(TelemetryReceiver))
                        .AddRuntimeInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddProcessInstrumentation()
                        .AddPrometheusExporter();
                });

            // configure logging ( serilog provider )
            // you can investigate issues on parametrice Serilog
            // configuration enable SelfLog.Enable(Console.Out); on 
            // the code ...

            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger());

            return services;
        }
    }
}
