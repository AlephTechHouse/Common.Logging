using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using ZephyrRelations.Logging.Elasticsearch.Extensions;
using ZephyrRelations.Logging.Seq.Extensions;
using ZephyrRelations.Logging.Extensions;

namespace ZephyrRelations.Logging.Extensions;

public static class IHostBuilderExtensions
{
    private const string SeqUrlKey = "Seq:Url";
    private const string ElasticsearchUrlKey = "Elasticsearch:Url";
    private const string ServiceNameKey = "ServiceSettings:ServiceName";

    private const string OutputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3} {SourceContext} {EventId}] {Message:lj} {Properties:j}{NewLine}{Exception}";
    public static IHostBuilder ConfigureZephyrRelationsLogging(this IHostBuilder hostBuilder, IConfiguration configuration)
    {

        return hostBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
        {
            var seqUrl = configuration[SeqUrlKey];
            var elasticsearchUrl = configuration[ElasticsearchUrlKey];
            var serviceName = configuration[ServiceNameKey];
            var environmentName = hostingContext.HostingEnvironment.EnvironmentName;

            loggerConfiguration
                .ConfigureSerilog(hostingContext.Configuration)
                .WriteToSeq(seqUrl!)
                .WriteToElasticsearch(elasticsearchUrl!, serviceName!, environmentName)
                .WriteTo.Console(
                    theme: AnsiConsoleTheme.Code,
                    outputTemplate: OutputTemplate)
                .WriteTo.Debug(outputTemplate: OutputTemplate);
        });
    }
}
