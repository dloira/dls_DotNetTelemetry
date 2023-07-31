# dls_DotNetTelemetry

Here, you could find an approach to face the observability patterns for a dotnet application ecosystem. The idea behind is to allow the teams for troubleshooting and helping them with the question, “Why and when is this happening?”. 

With the rise of cloud computing, microservices architectures, and ever-more complex business requirements, the need for Observability has never been greater. Observability is the ability to understand the internal state of a system by examining its outputs.

OpenTelemetry has become the industry standard and it will be the mechanism by which application code is instrumented in the way to fire signals such as traces, metrics, and logs. An application will be properly instrumented when it will not need to add anything else to troubleshoot an issue.

The code was built in .NET Core 6.0 using the official .NET SDK 7.0.201. To make that easier, the toolkit around the application (Grafana, Loki, Jaeger, Prometheus and SQL Server) will be setup using docker containers technology.

## Getting Started

The idea behind in this repository is to expose an API endpoint for retrieving a basic common weather forecast; to achieve telemetry squeezing, the logic inside is using some other external services like a public address API and MSSQL database. 

Out of the box .NET gives diagnostics and instrumentation with OpenTelemetry full integrated; take a look for a while in this Microsoft official link https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel

Please, find below the concept diagram for overview better understanding.

![Concept diagram](https://github.com/dloira/dls_DotNetTelemetry/blob/master/concept_diagram.jpg)

### Solution scaffolding

Please find below the most relevant folders for landing at ease in the source code. As you could check, the solution was built with **Central Package Management**; take a look for a while in this Microsoft official link https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management):

> **files** <br>
> &nbsp; Directory.Build.props *--> Common build configuration*<br>
> &nbsp; Directory.Packages.props *--> Central package management file*

> **src** <br>
> &nbsp; Telemetry_Receiver *--> Project application*

> **test** <br>
> &nbsp; Telemetry_RecieverFunctionalTests *--> Project to test the Project application features*

Drilling down the **Telemetry_Receiver** project the following folder tree was built. As you could ckeck, the project is based on the dependencies injection principles and burning the extension methods DotNet feature; take a look in this link for a better understaning https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods:

> **Progam.cs** *--> Starting point where the AspNet application is built and services added to the container*<br>
> **Extensions** *--> Services logic over extension methods*<br>
> **Diagnostics** *--> Metrics and logging deffinition*<br>
> **Fetures** *--> Weather forecast endpoint API logic*<br>
> **Infraestructure** *--> Utils for reading config file with database queries*<br>
> **Resources** *--> Database queries file*<br>
> **Options** *--> Options pattern to type settings parameters*<br>

It is important to remark how was improved the logging performance with source generator feature; in DotNet 6 you can create a partial method, decorate it with **[LoggerMessage]** attribute and the source generator will automatically fill in the log. Look into Diagnostics folder to know how it was build or read the Microsoft official link https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator.

### Adding OpenTelemetry

1. Setup the OpenTelemetry library within **AddObservability** method in **ServiceCollectionExtensions** static class:

```c#
// add instrumentation and metrics
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
            .AddSqlClientInstrumentation(options =>
            {
                options.RecordException = hostingEnvironment.IsDevelopment();
                options.SetDbStatementForText = hostingEnvironment.IsDevelopment();
            })
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
loggingBuilder.ClearProviders();
loggingBuilder.AddSerilog(new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .CreateLogger());
```
2. Create the Metric for the project, define the Measures in **TelemetryReceiverDiagnostics** public class:

```c#
private readonly Meter _receiverMeter;

private Counter<long> _httpEventProcessingExceptions = default!;
private Counter<long> _httpEventProcessingCount = default!;
private Histogram<long> _httpEventProcessingTime = default!;

...

_receiverMeter = new Meter(nameof(TelemetryReceiver));

...

private void InitializeCounters(string environmentName)
{
    _httpEventProcessingCount = _receiverMeter.CreateCounter<long>(
        name: TelemetryReceiverConstants.HTTP_EVENT_PROCESSING_COUNT_METRIC_NAME,
        description: TelemetryReceiverConstants.HTTP_EVENT_PROCESSING_COUNT_METRIC_DESCRIPTION);

    _httpEventProcessingExceptions = _receiverMeter.CreateCounter<long>(
        name: TelemetryReceiverConstants.HTTP_EVENT_PROCESSING_EXCEPTIONS_METRIC_NAME,
        description: TelemetryReceiverConstants.HTTP_EVENT_PROCESSING_EXCEPTIONS_METRIC_DESCRIPTION);

    _httpEventProcessingTime = _receiverMeter.CreateHistogram<long>(
        name: TelemetryReceiverConstants.HTTP_EVENT_PROCESSING_TIME_METRIC_NAME,
        description: TelemetryReceiverConstants.HTTP_EVENT_PROCESSING_TIME_METRIC_DESCRIPTION);

    _defaultTags = new KeyValuePair<string, object?>[]
    {
            new KeyValuePair<string, object?>(TelemetryReceiverConstants.COUNTER_TAG_ENVIRONMENT_NAME, environmentName),
            new KeyValuePair<string, object?>(TelemetryReceiverConstants.COUNTER_TAG_MACHINE_NAME, Environment.MachineName)
    };
}
```

3. Triggers the diagnostics methods to run the metrics, logs and traces in **GetWeatherForecastDataAsync** method within **WeatherForecastService** public class:

```c#
 public async Task<IEnumerable<GetWeatherForecastResponse>?> GetWeatherForecastDataAsync()
{
    _diagnostics.EventReceived();

    try
    {
        var watchProcesor = Stopwatch.StartNew();

        ...
        
        try
        {
            ...
        }
        catch (Exception exception)
        {
            _diagnostics.ErrorGettingWeatherForecast(exception);
        }
        finally
        {
            ...
        }

        _diagnostics.EventProcessed(watchProcesor.ElapsedMilliseconds, nameof(WeatherForecastService));

        ...
    }
    catch(Exception exception)
    {
        _diagnostics.EventProcessingFailed(exception);
    }

    ...
}
```

## Running the tests

Coming soon!

### Telemetry toolkit

Coming soon!

### Running the Telemetry_Receiver.FunctionalTests for XUnit checks

Coming soon!

### Running the Telemetry_Receiver from Visual Studio

Coming soon!

### Running the Telemetry_Receiver from Docker container

Coming soon!


## Built With

* [.Net-6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) - The .Net toolkit framework
* [VisualStudio-22](https://visualstudio.microsoft.com/es/vs/community/) - IDE
* [Docker-20.10.13](https://www.docker.com/) - Containers
* [Newtonsoft.Json-13.0.3"]() - 
* [Microsoft.Data.SqlClient-5.1.0"]() - MSSQL client
* [OpenTelemetry.Exporter.Console-1.4.0"]() - 
* [OpenTelemetry.Exporter.Jaeger-1.4.0"]() - 
* [OpenTelemetry.Exporter.Prometheus-1.3.0-rc.2"]() - 
* [OpenTelemetry.Exporter.Prometheus.AspNetCore-1.4.0-rc.4"]() - 
* [OpenTelemetry.Extensions.Hosting-1.4.0"]() - 
* [OpenTelemetry.Instrumentation.AspNetCore-1.0.0-rc9.14"]() - 
* [OpenTelemetry.Instrumentation.Http-1.0.0-rc9.14"]() - 
* [OpenTelemetry.Instrumentation.Process-0.5.0-beta.2"]() - 
* [OpenTelemetry.Instrumentation.Runtime-1.1.0-rc.2"]() - 
* [OpenTelemetry.Instrumentation.SqlClient-1.0.0-rc9.14"]() - 
* [Serilog.AspNetCore-6.1.0"]() - 
* [Serilog.Settings.Configuration-3.4.0"]() - 
* [Serilog.Sinks.Console-4.1.0"]() - 
* [Serilog.Sinks.File-5.0.0"]() - 
* [Serilog.Sinks.Grafana.Loki-8.1.0"]() - 
* [Swashbuckle.AspNetCore-6.5.0"]() - 
* [Dapper2.0.123"]() - 


## Versioning

I use [SemVer](http://semver.org/) for versioning. For the versions available, see the tags on this repository. 
