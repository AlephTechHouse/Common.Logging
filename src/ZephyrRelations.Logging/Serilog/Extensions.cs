// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Hosting;
// using Serilog;
// using Serilog.Events;
// using Serilog.Exceptions;
// using Serilog.Formatting.Compact;
// using Serilog.Formatting.Elasticsearch;
// using Serilog.Sinks.Elasticsearch;
// using Serilog.Sinks.SystemConsole.Themes;

// namespace Common.Logging.Serilogg;

// public static class Extensions
// {
//     private const string ElasticSearchUrlKey = "ElasticSearchUrl";
//     private const string SeqUrlKey = "SeqUrl";
//     private const string ServiceNameKey = "ServiceSettings:ServiceName";

//     public static IHostBuilder UseSerilogWithElasticsearch(
//         this IHostBuilder hostBuilder,
//         IConfiguration configuration)
//     {
//         var elasticsearchUrl = configuration[ElasticSearchUrlKey];
//         var seqUrl = configuration[SeqUrlKey];
//         var serviceName = configuration[ServiceNameKey];

// #pragma warning disable CS8604 // Possible null reference argument.
//         bool isSeqUrlConfigured = IsConfigKeySet(seqUrl),
//         isElasticsearchUrlConfigured = IsConfigKeySet(elasticsearchUrl),
//         isServiceNameConfigured = IsConfigKeySet(serviceName);
// #pragma warning restore CS8604 // Possible null reference argument.

//         hostBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
//         {
//             string? environmentName = configuration["ASPNETCORE_ENVIRONMENT"];

//             loggerConfiguration
//                 .ReadFrom.Configuration(configuration)
//                 .Enrich.FromLogContext()
//                 .Enrich.WithCorrelationId()
//                 .Enrich.WithMachineName()
//                 .Enrich.WithProcessId()
//                 .Enrich.WithThreadId()
//                 .Enrich.WithExceptionDetails()
//                 .Enrich.WithEnvironmentName()
//                 .WriteTo.Console(
//                     theme: AnsiConsoleTheme.Code,
//                     outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
//                 .WriteTo.Debug(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

//             if (isSeqUrlConfigured)
//             {
//                 loggerConfiguration.WriteTo.Seq(seqUrl!);
//             }

//             if (isElasticsearchUrlConfigured && isServiceNameConfigured)

//             {
//                 loggerConfiguration.WriteTo.Elasticsearch(CreateElasticsearchSinkOptions(elasticsearchUrl!, serviceName!, environmentName!));
//                 loggerConfiguration.WriteTo.Logger(
//                     lc =>
//                         lc.MinimumLevel.Information()
//                         .WriteTo.Console()
//                         .WriteTo.Debug()
//                         .WriteTo.Console(outputTemplate: $"Elasticsearch is active. Index =  {CreateIndexFormat(serviceName!, environmentName!)}"));

//             }
//         });

//         var tempLogger = new LoggerConfiguration()
//             .WriteTo.Console()
//             .CreateLogger();

//         LogConfigurationErrors(tempLogger, isSeqUrlConfigured, isElasticsearchUrlConfigured, isServiceNameConfigured);

//         return hostBuilder;
//     }

//     private static bool IsConfigKeySet(string configValue)
//     {
//         return !string.IsNullOrWhiteSpace(configValue);
//     }

//     private static void LogConfigurationErrors(
//         ILogger tempLogger,
//         bool isSeqUrlConfigured,
//         bool isElasticsearchUrlConfigured,
//         bool isServiceNameConfigured)
//     {
//         if (!isSeqUrlConfigured)
//         {
//             tempLogger.Error($"{nameof(SeqUrlKey)} is not configured in appsettings.json");
//         }

//         if (!isElasticsearchUrlConfigured)
//         {
//             tempLogger.Error($"{nameof(ElasticSearchUrlKey)} is not configured in appsettings.json");

//         }

//         if (!isServiceNameConfigured)
//         {
//             tempLogger.Error($"{nameof(ServiceNameKey)} is not configured in appsettings.json");
//         }

//     }
//     private static ElasticsearchSinkOptions CreateElasticsearchSinkOptions(string elasticsearchUrl, string serviceName, string environmentName)
//     {
//         var templateName = $"{serviceName}-logs-template";
//         var indexFormat = CreateIndexFormat(serviceName, environmentName);

//         return new ElasticsearchSinkOptions(new Uri(elasticsearchUrl))
//         {
//             AutoRegisterTemplate = true,
//             OverwriteTemplate = true,
//             TemplateName = templateName,
//             AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
//             IndexFormat = indexFormat,
//             CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
//             TypeName = null,
//             BatchAction = ElasticOpType.Create,
//             NumberOfReplicas = 0
//         };
//     }

//     private static string CreateIndexFormat(string serviceName, string environmentName)
//     {
//         return $"{serviceName.ToLower()}-logs-{environmentName.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy.MM}";
//     }
// }






