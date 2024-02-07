using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Storage;

namespace NewsMix.Services;

public class NotificationTasksExecutor : BackgroundService
{
    private readonly SqliteContext _context;
    private readonly IEnumerable<UserInterface> _userInterfaces;
    private readonly ILogger<NotificationTasksExecutor> _logger;

    public NotificationTasksExecutor(IServiceProvider services)
    {
        var scope = services.CreateScope(); //todo test disposing scope 
        _context = scope.ServiceProvider.GetRequiredService<SqliteContext>();
        _userInterfaces = scope.ServiceProvider.GetRequiredService<IEnumerable<UserInterface>>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<NotificationTasksExecutor>>();
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            var usersToNotify = await _context.Users
                .Include(u => u.NotificationTasks.Where(t => t.DoneAtUTC.HasValue == false))
                .Where(u => u.NotificationTasks.Any() == true)
                .ToListAsync(cancellationToken);

            foreach (var user in usersToNotify)
            {
                foreach (var task in user.NotificationTasks.ToArray())
                {
                    var userInterface = _userInterfaces.FirstOrDefault(i => i.UIName == user.UIType);
                    if (userInterface != null)
                    {
                        var text = task.HashTag switch
                        {
                            null => task.Url,
                            not null => task.HashTag + Environment.NewLine + task.Url
                        };

                        await userInterface.NotifyUser(user.ExternalUserId, text);

                        _logger?.LogWarning("Notified user {user}, publication {task}", user, task);

                        task.DoneAtUTC = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(30));
        }
    }
}