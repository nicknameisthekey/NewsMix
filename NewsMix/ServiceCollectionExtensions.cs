using Microsoft.Extensions.DependencyInjection;
using NewsMix.Abstractions;
using NewsMix.Feeds;
using NewsMix.Services;
using NewsMix.Storage;
using NewsMix.UI.Telegram;

namespace NewsMix;
public static class SerivceCollectionExtensions
{
    public static void AddNewsMix(this IServiceCollection services, bool addHosted)
    {
        if (addHosted)
        {
            services.AddHostedService<FeedService>();
        }

        services.AddTransient<UserService>();
        services.AddSingleton<Feed, NoobClubFeed>();
        services.AddSingleton<UserInterface, TelegramUI>();
        services.AddSingleton<ITelegramApi, TelegramApi>();
        services.AddSingleton<UserRepository, FileRepository>();
        services.AddSingleton<PublicationRepository, FileRepository>();
        services.AddSingleton<FeedsInformation, FeedsInformationService>();
        services.AddSingleton<DataDownloader, DataDownloaderService>();
        services.AddHttpClient();
    }
}