using Microsoft.Extensions.Logging;

public static class LoggingExtensions
{
    public static void LogItemsFetched(this ILogger? logger, int itemsAmout, string feedName)
                => logger?.LogWarning("Fetched {itemsAmount} from {feedName}", itemsAmout, feedName);
}