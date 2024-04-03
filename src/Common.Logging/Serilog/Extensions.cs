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
    public static IHostBuilder UseSerilogWithElasticsearch(
        this IHostBuilder hostBuilder,
        IConfiguration configuration)
    {
        var elasticserchUrl = configuration["ElasticSearchUrl"];
        var seqUrl = configuration["SeqUrl"];
        var applicationName = configuration["ApplicationName"];

        if (string.IsNullOrWhiteSpace(elasticserchUrl) || string.IsNullOrWhiteSpace(seqUrl) || string.IsNullOrWhiteSpace(applicationName))
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Error("ElasticSearchUrl or SeqUrl or applicationName is not configured in appsettings.json");
            return hostBuilder;
        }

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.WithExceptionDetails()
            .Enrich.WithEnvironmentName()
            .WriteTo.Console(
                theme: AnsiConsoleTheme.Code,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
            )
            .WriteTo.Debug(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.Seq(seqUrl)
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticserchUrl))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                IndexFormat = $"{applicationName.ToLower()}-{DateTime.UtcNow:yyyy.MM}",
                CustomFormatter = new ElasticsearchJsonFormatter(renderMessage: true, inlineFields: true)
            })
            .CreateLogger();

        hostBuilder.UseSerilog();

        return hostBuilder;
    }
}






