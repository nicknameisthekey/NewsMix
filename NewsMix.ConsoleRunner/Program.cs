using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace NewsMix.ConsoleRunner;

public class Program
{
    public static async Task Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((ctx, services) =>
            {
                services.AddNewsMix(ctx.Configuration, true);

                services.Configure<HostOptions>(options => { options.ShutdownTimeout = TimeSpan.FromSeconds(2); });
            })
            .UseSerilog((ctx, conf) =>
            {
                conf.ReadFrom.Configuration(ctx.Configuration);
            })
            .Build();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Service started");

        await host.RunAsync();
    }
}