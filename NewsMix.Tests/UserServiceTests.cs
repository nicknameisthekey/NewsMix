using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsMix.Models;
using NewsMix.Services;
using NewsMix.Storage;
using NewsMix.Storage.Entities;
using static NewsMix.Tests.TestHelpers;

namespace NewsMix.Tests;

public class UserServiceTests
{
    private readonly IConfiguration config = TestHelpers.PrepareTestEnv();
    UserModel TestUser => new() { ExternalUserId = "12345", UIType = "telegram", Name = "1234" };
    Subscription TestSub => new() { Source = "source", TopicInternalName = "topic" };

    [Fact]
    public async Task Adding_subscription_to_non_existing_user_creates_new_user_with_subscription()
    {
        var services = new ServiceCollection()
            .AddNewsMix(config, false)
            .CreateScope();
        var repo = services.GetRequiredService<SqliteRepository>();
        var userService = services.GetRequiredService<UserService>();

        await userService.AddSubscription(TestUser, TestSub);

        var user = await repo.GetOrCreate(TestUser);
        Assert.Single(user.Subscriptions);
    }

    [Fact]
    public async Task Duplication_of_subscriptions_prevented()
    {
        var services = new ServiceCollection()
            .AddNewsMix(config, false)
            .CreateScope();
        var repo = services.GetRequiredService<SqliteRepository>();
        var userService = services.GetRequiredService<UserService>();

        await userService.AddSubscription(TestUser, TestSub);
        await userService.AddSubscription(TestUser, TestSub);

        var user = await repo.GetOrCreate(TestUser);
        Assert.Single(user.Subscriptions);
    }

    [Fact]
    public async Task Adding_new_subscription_dont_remove_other_subscriptions()
    {
        var services = new ServiceCollection()
            .AddNewsMix(config, false)
            .CreateScope();
        var repo = services.GetRequiredService<SqliteRepository>();
        var userService = services.GetRequiredService<UserService>();

        await userService.AddSubscription(TestUser, TestSub);
        var someOtherSubscription = new Subscription("other sub", "other pub");
        await userService.AddSubscription(TestUser, someOtherSubscription);

        var user = await repo.GetOrCreate(TestUser);
        Assert.Equal(2, user.Subscriptions.Count);
        Assert.Contains(user.Subscriptions, s => s.SameAs(TestSub));
        Assert.Contains(user.Subscriptions, s => s.SameAs(someOtherSubscription));
    }

    [Fact]
    public async Task Subscription_can_be_removed()
    {
        var services = new ServiceCollection()
            .AddNewsMix(config, false)
            .CreateScope();
        var repo = services.GetRequiredService<SqliteRepository>();
        var userService = services.GetRequiredService<UserService>();

        var someOtherSubscription = new Subscription("other sub", "other pub");
        await userService.AddSubscription(TestUser, TestSub);
        await userService.AddSubscription(TestUser, someOtherSubscription);
        await userService.RemoveSubscription(TestUser, someOtherSubscription);

        var user = await repo.GetOrCreate(TestUser);
        Assert.Single(user.Subscriptions);
        Assert.True(user.Subscriptions[0].SameAs(TestSub));
    }

    [Fact]
    public async Task Removing_not_existing_subscription_do_nothing()
    {
        var services = new ServiceCollection()
            .AddNewsMix(config, false)
            .CreateScope();
        var repo = services.GetRequiredService<SqliteRepository>();
        var userService = services.GetRequiredService<UserService>();

        var someOtherSubscription = new Subscription("other sub", "other pub");
        await userService.AddSubscription(TestUser, TestSub);
        await userService.RemoveSubscription(TestUser, someOtherSubscription);

        var user = await repo.GetOrCreate(TestUser);
        Assert.Single(user.Subscriptions);
        Assert.True(user.Subscriptions[0].SameAs(TestSub));
    }
}