using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using Telemetry_Receiver.Diagnostics;
using Telemetry_Receiver.Extensions;
using Telemetry_Receiver.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddObservability(builder.Logging, builder.Environment, builder.Configuration)
    .AddApplicationOptions(builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddVersioning()
    .AddOpenApi()
    .AddControllers();

// build the pipeline
var app = builder.Build();

app.UseOpenApi();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapPrometheusScrapingEndpoint();
});

app.Run();
