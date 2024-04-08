using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;

namespace Common.Logging.Serilog;

public static class Extensions
{
    private const string ElasticSearchUrlKey = "ElasticSearchUrl";
    private const string SeqUrlKey = "SeqUrl";
    private const string ServiceNameKey = "ServiceSettings:ServiceName";

    public static IHostBuilder UseSerilogWithElasticsearch(
        this IHostBuilder hostBuilder,
        IConfiguration configuration)
    {
        var elasticserchUrl = configuration[ElasticSearchUrlKey];
        var seqUrl = configuration[SeqUrlKey];
        var serviceName = configuration[ServiceNameKey];

        bool isSeqUrlConfigured = !string.IsNullOrWhiteSpace(seqUrl);
        bool isElasticsearchUrlConfigured = !string.IsNullOrWhiteSpace(elasticserchUrl);
        bool isServiceNameConfigured = !string.IsNullOrWhiteSpace(serviceName);

        var loggerConfiguration = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.WithExceptionDetails()
            .Enrich.WithEnvironmentName()
            .WriteTo.Console(
                theme: AnsiConsoleTheme.Code,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.Debug(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

        if (isSeqUrlConfigured)
        {
            loggerConfiguration.WriteTo.Seq(seqUrl!);
        }

        if (isElasticsearchUrlConfigured && isServiceNameConfigured)

        {
            loggerConfiguration.WriteTo.Elasticsearch(CreateElasticsearchSinkOptions(elasticserchUrl!, serviceName!));
        }

        Log.Logger = loggerConfiguration.CreateLogger();

        if (!isSeqUrlConfigured)
        {
            Log.Logger.Error($"{nameof(SeqUrlKey)} is not configured in appsettings.json");
        }

        if (!isElasticsearchUrlConfigured)
        {
            Log.Logger.Error($"{nameof(ElasticSearchUrlKey)} is not configured in appsettings.json");

        }

        if (!isServiceNameConfigured)
        {
            Log.Logger.Error($"{nameof(ServiceNameKey)} is not configured in appsettings.json");
        }
        return hostBuilder;
    }
    private static ElasticsearchSinkOptions CreateElasticsearchSinkOptions(string elasticserchUrl, string serviceName)
    {
        return new ElasticsearchSinkOptions(new Uri(elasticserchUrl))
        {

            AutoRegisterTemplate = true,
            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
            IndexFormat = $"{serviceName!.ToLower()}-{DateTime.UtcNow:yyyy.MM}",
            CustomFormatter = new ElasticsearchJsonFormatter(renderMessage: true, inlineFields: true)
        };
    }
}






