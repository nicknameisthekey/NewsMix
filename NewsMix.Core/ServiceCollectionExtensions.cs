using Microsoft.Extensions.DependencyInjection;
using NewsMix.Core.Services;
using NewsMix.UI;

namespace NewsMix.Core;
public static class SerivceCollectionExtensions
{
    public static void AddCoreServices(this IServiceCollection services)
    {
        services.AddHostedService<FeedService>();
        services.AddTransient<UserService>();
        services.AddFeeds();
        services.AddUI();
    }
}