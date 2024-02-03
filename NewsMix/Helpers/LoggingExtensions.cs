using Microsoft.Extensions.Logging;

namespace NewsMix.Helpers;

public static class LoggingExtensions
{
    public static void LogPublicationsFetched(this ILogger? logger, int itemsAmount, string sourceName)
        => logger?.LogWarning("Fetched {itemsAmount} from {sourceName}", itemsAmount, sourceName);
}