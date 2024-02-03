using Microsoft.Extensions.DependencyInjection;
using NewsMix.Abstractions;
using NewsMix.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NewsMix.NewsSources;
using NewsMix.Storage;
using Telegram.Bot;

namespace NewsMix;
public static class ServiceCollectionExtensions
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
        services.AddScoped<NewsSource, NoobClub>();
        services.AddScoped<IStatsService, StatsService>();
        services.AddScoped<NewsSource, IcyVeins>();
        services.AddScoped<NewsSource, EaApex>();
        services.AddScoped<NewsSource, Habr>();
        services.AddSingleton<UserInterface, UI.Telegram.Telegram>();
        services.AddScoped<UserRepository, SqliteRepository>();
        services.AddScoped<PublicationsRepository, SqliteRepository>();
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