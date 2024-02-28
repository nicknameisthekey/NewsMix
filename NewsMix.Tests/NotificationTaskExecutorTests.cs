using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsMix.Abstractions;
using NewsMix.Services;
using NewsMix.Storage;
using NewsMix.Storage.Entities;
using NewsMix.Tests.Mocks;

namespace NewsMix.Tests;

public class NotificationTaskExecutorTests
{
    private readonly IConfiguration config = TestHelpers.PrepareTestEnv();

    [Fact]
    public async Task No_notification_if_subscription_does_not_exist()
    {
        var fakeTelegram = new FakeTelegramUI()
        {
            OnNotify = (_, _, _) => throw new Exception()
        };
        var services = new ServiceCollection()
            .AddNewsMix(config, false)
            .AddHostedService<NotificationTasksExecutor>()
            .WithScopedMock<UserInterface, FakeTelegramUI>(fakeTelegram)
            .CreateScope();

        var executor = services.GetHostedService<NotificationTasksExecutor>();
        var ctx = services.GetRequiredService<SqliteContext>();

        ctx.Users.Add(new User
        {
            Id = 1,
            ExternalUserId = "abc",
            Name = "pepega",
            UIType = fakeTelegram.UIName,
            CreatedAt = default,
            UserActions = [],
            Subscriptions = [],
            NotificationTasks = new List<NotificationTask>()
            {
                new NotificationTask
                {
                    Id = 1,
                    InternalUserId = 1,
                    Url = "",
                    NewsSource = "",
                    TopicInternalName = "",
                    HashTag = "",
                    DoneAtUTC = null,
                    CreatedAtUTC = default,
                }
            }
        });

        ctx.SaveChanges();

        await executor.JobBody(default);
    }
    
    [Fact]
    public async Task Notification_sent_only_once()
    {
        var fakeTelegram = new FakeTelegramUI()
        {
            OnNotify = (_, _, _) => { }
        };
        var services = new ServiceCollection()
            .AddNewsMix(config, false)
            .AddHostedService<NotificationTasksExecutor>()
            .WithScopedMock<UserInterface, FakeTelegramUI>(fakeTelegram)
            .CreateScope();

        var executor = services.GetHostedService<NotificationTasksExecutor>();
        var ctx = services.GetRequiredService<SqliteContext>();
    
        ctx.Users.Add(new User
        {
            Id = 1,
            ExternalUserId = "abc",
            Name = "pepega",
            UIType = fakeTelegram.UIName,
            CreatedAt = default,
            UserActions = [],
            Subscriptions = [new Subscription
                {
                    Id = 1,
                    InternalUserId = 1,
                    Source = "s",
                    TopicInternalName = "t",
                    CreatedOnUTC = default,
                }
            ],
            NotificationTasks = new List<NotificationTask>()
            {
                new NotificationTask
                {
                    Id = 1,
                    InternalUserId = 1,
                    Url = "",
                    NewsSource = "s",
                    TopicInternalName = "t",
                    HashTag = "",
                    DoneAtUTC = null,
                    CreatedAtUTC = default,
                }
            }
        });

        ctx.SaveChanges();

        var task = ctx.NotificationTasks.AsNoTracking().Single();
        Assert.Null(task.DoneAtUTC);

        await executor.JobBody(default);
        task = ctx.NotificationTasks.AsNoTracking().Single();
        Assert.NotNull(task.DoneAtUTC);
        
        fakeTelegram.OnNotify = (_, _, _) => throw new Exception();
        await executor.JobBody(default);
    }
}