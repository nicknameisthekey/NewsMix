using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsMix.Core;
using NewsMix.DAL;

namespace NewsMix.ConsoleRunner;
public class Program
{
    public static async Task Main()
    {
        var host = Host.CreateDefaultBuilder()
                       .ConfigureServices(s =>
                          {
                              s.AddFileRepository();
                              s.AddCoreServices();
                              s.AddLogging(b => b
                                  .AddDebug()
                                  .AddConsole());
                          }).Build();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Host created.");
        await host.RunAsync();
    }
}
