using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NewsMix.Feeds;
using NewsMix.Feeds.FeedImplementations;

public static class ServiceCollectionExtensions
{
    public static void AddFeeds(this IServiceCollection services)
    {
        services.AddSingleton<Feed, NoobClubFeed>();
        services.AddHttpClient();
    }
}