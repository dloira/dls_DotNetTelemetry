using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Telemetry_Receiver.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseOpenApi(this IApplicationBuilder app)
        {
            return app.UseSwagger()
                .UseSwaggerUI(options =>
                {
                    var provider = app.ApplicationServices
                        .GetRequiredService<IApiVersionDescriptionProvider>();

                    foreach (var apiVersionDescription in provider.ApiVersionDescriptions.OrderByDescending(x => x.ApiVersion))
                    {
                        options.SwaggerEndpoint(
                            $"{apiVersionDescription.GroupName}/swagger.json",
                            $"Version {apiVersionDescription.ApiVersion}");
                    }
                });
        }
    }
}
