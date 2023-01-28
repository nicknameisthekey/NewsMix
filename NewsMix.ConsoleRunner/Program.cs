using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

                              s.Configure<HostOptions>(options =>
                                {
                                    options.ShutdownTimeout = TimeSpan.FromSeconds(2);
                                });
                          }).Build();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Service started");

        await host.RunAsync();
    }
}
