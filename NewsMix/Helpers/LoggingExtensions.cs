using Microsoft.Extensions.Logging;

public static class LoggingExtensions
{
    public static void LogPublicationsFetched(this ILogger? logger, int itemsAmout, string sourceName)
                => logger?.LogWarning("Fetched {itemsAmount} from {sourceName}", itemsAmout, sourceName);
}