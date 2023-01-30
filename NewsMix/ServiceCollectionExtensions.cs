using Microsoft.Extensions.DependencyInjection;
using NewsMix.Abstractions;
using NewsMix.Sources;
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
            services.AddHostedService<TelegramUI>();
            services.AddHostedService<SourcesService>();
            services.AddHostedService<BotChangesNotifier>();
        }

        services.AddTransient<UserService>();
        services.AddSingleton<Source, NoobClub>();
        services.AddSingleton<Source, IcyVeins>();
        services.AddSingleton<Source, EaApex>();
        services.AddSingleton<Source, Habr>();
        services.AddSingleton<UserInterface, TelegramUI>();
        services.AddSingleton<ITelegramApi, TelegramApi>();
        services.AddSingleton<UserRepository, FileRepository>();
        services.AddSingleton<PublicationRepository, FileRepository>();
        services.AddSingleton<SourcesInformation, SourcessInformationService>();
        services.AddSingleton<DataDownloader, DataDownloaderService>();
        services.AddHttpClient();
    }
}