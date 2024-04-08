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
        var serviceName = configuration["ServiceSettings:ServiceName"];

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

        if (!string.IsNullOrWhiteSpace(seqUrl))
        {
            loggerConfiguration.WriteTo.Seq(seqUrl);
        }
        else
        {
            Log.Logger.Error("SeqUrl is not configured in appsettings.json");
        }

        if (string.IsNullOrWhiteSpace(elasticserchUrl))
        {
            Log.Logger.Error("ElasticSearchUrl is not configured in appsettings.json");
        }
        else if (string.IsNullOrWhiteSpace(serviceName))
        {
            Log.Logger.Error("ServiceSettings:ServiceName is not configured in appsettings.json");
        }
        else
        {
            loggerConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticserchUrl))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                IndexFormat = $"{serviceName.ToLower()}-{DateTime.UtcNow:yyyy.MM}",
                CustomFormatter = new ElasticsearchJsonFormatter(renderMessage: true, inlineFields: true)
            });
        }

        Log.Logger = loggerConfiguration.CreateLogger();
        return hostBuilder;
    }
}






