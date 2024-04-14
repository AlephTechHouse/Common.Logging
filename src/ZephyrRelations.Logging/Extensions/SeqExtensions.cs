using ZephyrRelations.Logging.Services;
using Serilog;

namespace ZephyrRelations.Logging.Seq.Extensions;

public static class SeqExtensions
{
    public static LoggerConfiguration WriteToSeq(this LoggerConfiguration loggerConfiguration, string seqUrl)
    {

        var loggingService = new LoggingService();

        if (string.IsNullOrWhiteSpace(seqUrl))
        {
            loggingService.LogWarning("Seq URL is not configured correctly.");
        }
        else
        {
            loggerConfiguration.WriteTo.Seq(seqUrl);
            loggingService.LogInformation("Seq configuration is active.");
        }
        return loggerConfiguration;
    }
}
