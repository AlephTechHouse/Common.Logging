using ZephyrRelations.Logging.Constants;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ZephyrRelations.Logging.Services;

public class LoggingService
{

    private readonly Serilog.ILogger _logger;

    public LoggingService()
    {
        _logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: LoggingConstants.OutputTemplate)
            .CreateLogger();
    }

    public void LogInformation(string message)
    {
        _logger.Information(message);
    }
    public void LogWarning(string message)
    {
        _logger.Warning(message);
    }

    public void LogError(string message)
    {
        _logger.Error(message);
    }

}
