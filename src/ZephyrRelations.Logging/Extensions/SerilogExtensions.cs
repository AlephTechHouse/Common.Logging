using ZephyrRelations.Logging.Constants;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.SystemConsole.Themes;

namespace ZephyrRelations.Logging.Extensions;

public static class SerilogExtensions
{
    public static LoggerConfiguration ConfigureSerilog(this LoggerConfiguration loggerConfiguration, IConfiguration configuration)
    {
        return loggerConfiguration
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationId()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.WithExceptionDetails()
            .Enrich.WithEnvironmentName()
                .WriteTo.Console(
                    theme: AnsiConsoleTheme.Code,
                    outputTemplate: LoggingConstants.OutputTemplate)
                .WriteTo.Debug(outputTemplate: LoggingConstants.OutputTemplate);
    }
}
