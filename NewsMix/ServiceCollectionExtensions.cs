using Microsoft.Extensions.DependencyInjection;
using NewsMix.Abstractions;
using NewsMix.Feeds;
using NewsMix.Services;
using NewsMix.Storage;
using NewsMix.UI.Telegram;

namespace NewsMix;
public static class SerivceCollectionExtensions
{
    public static void AddNewsMix(this IServiceCollection services)
    {
        services.AddHostedService<FeedService>();
        services.AddTransient<UserService>();
        services.AddSingleton<Feed, NoobClubFeed>();
        services.AddSingleton<TelegramUI>();
        services.AddSingleton<UserRepository, FileRepository>();
        services.AddSingleton<PublicationRepository, FileRepository>();
        services.AddHttpClient();
    }
}