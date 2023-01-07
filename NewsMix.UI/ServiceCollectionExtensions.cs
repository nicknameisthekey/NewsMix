using Microsoft.Extensions.DependencyInjection;
using NewsMix.UI.Telegram;

namespace NewsMix.UI;

public static class ServiceCollectionExtensions
{
    public static void AddUI(this IServiceCollection services)
    {
        services.AddSingleton<UserInterface, TelegramUI>();
        services.AddHttpClient();
    }
}