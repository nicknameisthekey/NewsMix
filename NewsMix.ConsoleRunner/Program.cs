using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsMix.UI.Telegram;

namespace NewsMix.ConsoleRunner;
public class Program
{
    public static async Task Main()
    {
        var host = Host.CreateDefaultBuilder()
                       .ConfigureServices(s =>
                          {
                              s.AddNewsMix(true);
                              s.AddLogging(b => b
                                  .AddDebug()
                                  .AddConsole());
                          }).Build();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Host created.");


        var tUI = host.Services.GetRequiredService<TelegramUI>();
        // await tUI.ProcessTextMessage(new UI.Telegram.Models.Message
        // {
        //     Sender = new UI.Telegram.Models.User
        //     {
        //         Id = 303656773
        //     },
        //     Text = "/start"
        // });

        await host.RunAsync();
    }
}
