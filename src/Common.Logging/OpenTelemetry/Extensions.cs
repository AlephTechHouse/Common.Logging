using Microsoft.Extensions.Configuration;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Common.Logging.OpenTelemetry;

public static class Extensions
{
    public static TracerProviderBuilder UseOpenTelemetry(
        this TracerProviderBuilder builder,
        IConfiguration configuration,
        string serviceName)
    {
        var configErrorLogger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        var jaegerUrl = configuration["JaegerUrl"];
        var jaegerPort = configuration["JaegerPort"];

        if (string.IsNullOrWhiteSpace(jaegerUrl) || string.IsNullOrWhiteSpace(jaegerPort))
        {
            configErrorLogger.Error("JaegerUrl or JaegerPort is not configured in appsettings.json");
            throw new ArgumentNullException("JaegerUrl or JaegerPort is not configured in appsettings.json");
        }

        try
        {
            builder.AddJaegerExporter(options =>
            {
                options.AgentHost = jaegerUrl;
                options.AgentPort = int.Parse(jaegerPort);
            })
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddSource(serviceName)
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName));
        }
        catch (Exception ex)
        {
            configErrorLogger.Error(ex, "Error configuring OpenTelemetry with Jaeger");
            throw;
        }

        return builder;
    }
}
