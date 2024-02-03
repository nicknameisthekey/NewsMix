using Microsoft.Extensions.DependencyInjection;
using NewsMix.Abstractions;
using NewsMix.Sources;
using NewsMix.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;

namespace NewsMix;
public static class SerivceCollectionExtensions
{
    public static void AddNewsMix(this IServiceCollection services, IConfiguration config, bool addHosted)
    {
        services.AddDbContext<SqliteContext>
            (o => o.UseSqlite(config.GetConnectionString("sqlite")));

        if (addHosted)
        {
            services.AddHostedService<UI.Telegram.Telegram>();
            services.AddHostedService<SourcesService>();
        }

        services.AddScoped<UserService>();
        services.AddScoped<Source, NoobClub>();
        services.AddScoped<IStatsService, StatsService>();
        services.AddScoped<Source, IcyVeins>();
        services.AddScoped<Source, EaApex>();
        services.AddScoped<Source, Habr>();
        services.AddSingleton<UserInterface, UI.Telegram.Telegram>();
        services.AddScoped<UserRepository, SqliteRepository>();
        services.AddScoped<PublicationRepository, SqliteRepository>();
        services.AddScoped<SourcesInformation, SourcessInformationService>();
        services.AddScoped<DataDownloader, DataDownloaderService>();

        services.AddSingleton<ITelegramBotClient>(s =>
        {
            var token =  config["TelegramBotToken"] ?? throw new Exception("TelegramBotToken");

            var client =  new TelegramBotClient(token);
            return client;
        });
        services.AddHttpClient();
    }
}