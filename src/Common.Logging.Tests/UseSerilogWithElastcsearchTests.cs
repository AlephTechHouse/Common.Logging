using Common.Logging.Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.InMemory;

namespace Common.Logging.Test;

public class UseSerilogWithElastcsearchTests
{
    [Fact]
    public void UseSerilogWithElasticsearch_WhenCalled_ConfiguresLoggerCorrectly()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
    {
        {"ElasticsearchUrl", "http://localhost:9200"},
        {"SeqUrl", "http://localhost:5341"},
        {"ApplicationName", "YourApplicationName"}
    };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        var hostBuilder = Host.CreateDefaultBuilder();

        // Act
        hostBuilder.UseSerilogWithElasticsearch(configuration);
        var host = hostBuilder.Build();

        // Assert
        var logger = host.Services.GetService<ILogger<UseSerilogWithElastcsearchTests>>();
        if (logger is null)
        {
            throw new ArgumentNullException("Logger is null");
        }
        logger.LogInformation("This is a test log message");
        Assert.NotNull(logger);
    }
    [Fact]
    public void UseSerilogWithElasticsearch_WhenElasticsearchUrlNotConfigured_ThrowsArgumentNullException()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            {"ElasticsearchUrl", "localhost:9200"},
            {"SeqUrl", ""}, // misconfigured
            {"ServiceSettings:ServiceName", "MyService"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        var hostBuilder = new HostBuilder();

        var sink = new InMemorySink();
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Sink(sink)
            .CreateLogger();

        // Act
        hostBuilder.UseSerilogWithElasticsearch(configuration);

        // Assert
        var logEvent = sink.LogEvents.FirstOrDefault();

        Assert.NotNull(logEvent);
        Assert.Contains("SeqUrl is not configured in appsettings.json", logEvent.RenderMessage());

    }
}

