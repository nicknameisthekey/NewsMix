using Microsoft.Extensions.DependencyInjection;
using NewsMix.Abstractions;
using NewsMix.Sources;
using NewsMix.Services;
using NewsMix.UI.Telegram;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace NewsMix;
public static class SerivceCollectionExtensions
{
    public static void AddNewsMix(this IServiceCollection services, IConfiguration config, bool addHosted)
    {
        services.AddDbContext<SqliteContext>
            (o => o.UseSqlite(config.GetConnectionString("sqlite")));

        if (addHosted)
        {
            services.AddHostedService<TelegramUI>();
            services.AddHostedService<SourcesService>();
        }


        services.AddScoped<UserService>();
        services.AddScoped<Source, NoobClub>();
        services.AddScoped<IStatsService, StatsService>();
        services.AddScoped<Source, IcyVeins>();
        services.AddScoped<Source, EaApex>();
        services.AddScoped<Source, Habr>();
        services.AddScoped<UserInterface, TelegramUI>();
        services.AddScoped<ITelegramApi, TelegramApi>();
        services.AddScoped<UserRepository, SqliteRepository>();
        services.AddScoped<PublicationRepository, SqliteRepository>();
        services.AddScoped<SourcesInformation, SourcessInformationService>();
        services.AddScoped<DataDownloader, DataDownloaderService>();
        services.AddHttpClient();
    }
}