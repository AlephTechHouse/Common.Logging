using Microsoft.Extensions.Configuration;
using ZephyrRelations.Logging.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Elastic.Clients.Elasticsearch;

namespace ZephyrRelations.Logging.Tests;

public class ElasticsearchLoggingTests
{

    [Fact]
    [Trait("SkipOnCI", "true")]
    public async Task LogsAreWrittenToElasticsearch()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "ElasticSearchUrl", "http://localhost:9200" },
            { "SeqUrl", "http://localhost:5341" },
            { "ServiceSettings:ServiceName", "TestService" },
            { "ASPNETCORE_ENVIRONMENT", "Development"}
            // Add other settings as needed
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var hostBuilder = new HostBuilder().ConfigureZephyrRelationsLogging(configuration);
        var host = hostBuilder.Build();
        var logger = host.Services.GetRequiredService<ILogger<ElasticsearchLoggingTests>>();

        // Act
        //global::Serilog.Debugging.SelfLog.Enable(Console.Error);
        logger.LogInformation("Test log message");

        // Allow some time for the log message to be written to Elasticsearch
        await Task.Delay(5000);

        // Assert
        var client = new ElasticsearchClient(new Uri("http://localhost:9200"));
        var searchResponse = await client.SearchAsync<LogEntry>(s => s
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Message) // Changed from RenderedMessage to Message
                    .Query("Test log message"))));

        Assert.True(searchResponse.Documents.Any());
    }

    private class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string? Level { get; set; }
        public string? MessageTemplate { get; set; }
        public string? RenderedMessage { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, object>? Properties { get; set; }
        public object? Exception { get; set; }
    }
}
