using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Storage;
using NewsMix.Storage.Entities;

namespace NewsMix.Services;

public class NotificationTasksExecutor : BackgroundService
{
    private readonly SqliteContext _context;
    private readonly IEnumerable<UserInterface> _userInterfaces;
    private readonly ILogger<NotificationTasksExecutor>? _logger;

    public NotificationTasksExecutor(IServiceProvider services)
    {
        var scope = services.CreateScope(); //todo test disposing scope 
        _context = scope.ServiceProvider.GetRequiredService<SqliteContext>();
        _userInterfaces = scope.ServiceProvider.GetRequiredService<IEnumerable<UserInterface>>();
        _logger = scope.ServiceProvider.GetService<ILogger<NotificationTasksExecutor>>();
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            try
            {
                await JobBody(cancellationToken);
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Error on processing notification tasks");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
        }
    }

    public async Task JobBody(CancellationToken cancellationToken)
    {
        var tasksToNotify = await _context.NotificationTasks
            .Include(t => t.User)
            .Where(t => t.DoneAtUTC.HasValue == false)
            .ToListAsync(cancellationToken);

        foreach (var task in tasksToNotify)
        {
            try
            {
                if (await SubscriptionActive(task)) 
                    await Notify(task.User, task);

                task.DoneAtUTC = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Error while processing {notificationTaskId} to {internalUserId}", task.Id,
                    task.User.Id);
            }
        }
    }

    private async Task<bool> SubscriptionActive(NotificationTask task)
    {
        return await _context.Subscriptions.AnyAsync
        (s => s.InternalUserId == task.InternalUserId &&
              s.TopicInternalName == task.TopicInternalName &&
              s.Source == task.NewsSource);
    }

    private async Task Notify(User user, NotificationTask task)
    {
        var userInterface = _userInterfaces.FirstOrDefault(i => i.UIName == user.UIType);
        if (userInterface != null)
        {
            var text = task.HashTag switch
            {
                null => task.Url,
                not null => task.HashTag + Environment.NewLine + task.Url
            };

            await userInterface.NotifyUser(user.ExternalUserId, text, task.Id);

            _logger?.LogWarning("Processed {notificationTaskId} {internalUserId}", task.Id, user.Id);
        }
    }
}