using ZephyrRelations.Logging.Services;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace ZephyrRelations.Logging.Elasticsearch.Extensions;

public static class ElasticsearchExtensions
{
    public static LoggerConfiguration WriteToElasticsearch(this LoggerConfiguration loggerConfiguration, string elasticsearchUrl, string serviceName, string environmentName)
    {
        var loggingService = new LoggingService();

        if (!string.IsNullOrWhiteSpace(elasticsearchUrl) && !string.IsNullOrWhiteSpace(serviceName))
        {
            loggerConfiguration.WriteTo.Elasticsearch(CreateElasticsearchSinkOptions(elasticsearchUrl, serviceName, environmentName));
            loggingService.LogInformation("Elasticsearch configuration is active.");
        }
        else
        {
            loggingService.LogWarning("Elasticsearch URL or service name is not configured correctly.");
        }
        return loggerConfiguration;
    }

    private static ElasticsearchSinkOptions CreateElasticsearchSinkOptions(string elasticsearchUrl, string serviceName, string environmentName)
    {
        var templateName = $"{serviceName}-logs-template";
        var indexFormat = CreateIndexFormat(serviceName, environmentName);

        return new ElasticsearchSinkOptions(new Uri(elasticsearchUrl))
        {
            AutoRegisterTemplate = true,
            OverwriteTemplate = true,
            TemplateName = templateName,
            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
            IndexFormat = indexFormat,
            CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
            TypeName = null,
            BatchAction = ElasticOpType.Create,
            NumberOfReplicas = 0
        };
    }

    private static string CreateIndexFormat(string serviceName, string environmentName)
    {
        return $"{serviceName.ToLower()}-logs-{environmentName.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy.MM}";
    }
}
