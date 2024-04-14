using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZephyrRelations.Logging.Extensions;
using Serilog;
using Serilog.Sinks.InMemory;

namespace ZephyrRelations.Logging.Test;

public class ConfigureZephyrRelationsLoggingTest
{
    [Fact]
    public void ConfigureZephyrRelationsLogging_WhenCalled_ConfiguresLoggerCorrectly()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
    {
        {"ElasticsearchUrl", "http://localhost:9200"},
        {"SeqUrl", "http://localhost:5341"},
        {"ServiceSettings:ServiceName", "YourServiceName"}
    };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        var hostBuilder = Host.CreateDefaultBuilder();

        // Act
        hostBuilder.ConfigureZephyrRelationsLogging(configuration);
        var host = hostBuilder.Build();

        // Assert
        var logger = host.Services.GetService<ILogger<ConfigureZephyrRelationsLoggingTest>>();
        if (logger is null)
        {
            throw new ArgumentNullException("Logger is null");
        }
        logger.LogInformation("This is a test log message");
        Assert.NotNull(logger);
    }

}
