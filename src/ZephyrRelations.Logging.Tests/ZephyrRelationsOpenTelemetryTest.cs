using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using ZephyrRelations.OpenTelemetry.Extensions;

namespace ZephyrRelations.Logging.Tests;

public class ZephyrRelationsOpenTelemetryTest
{
    [Fact]
    public void UseOpenTelemetry_AddsServicesToCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jaeger:Url", "http://localhost:14250" },
                { "Jaeger:Port", "1234" },
                { "Prometheus:EndpointPath", "/metrics" },
                { "ServiceSettings:ServiceName", "TestService"}
            })
            .Build();

        // Act
        services.UseOpenTelemetry(configuration);

        // Assert
        Assert.Contains(services, service => service.ServiceType == typeof(TracerProvider));
        Assert.Contains(services, service => service.ServiceType == typeof(MeterProvider));
    }
}
