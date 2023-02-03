using NewsMix.Models;
using NewsMix.Services;
using NewsMix.Storage.Entites;
using static TestHelpers;

public class UserServiceTests
{
    UserModel TestUser => new UserModel { UserId = "12345", UIType = "telegram", Name = "1234" };
    Subscription TestSub => new Subscription { Source = "source", Topic = "topic" };

    [Fact]
    public async Task Adding_subscription_to_non_existing_user_creates_new_user_with_subscription()
    {
        var (repo, ctx) = CreateDb();
        var userSerivce = new UserService(repo);

        await userSerivce.AddSubscription(TestUser, TestSub);

        var user = await repo.GetOrCreate(TestUser);
        Assert.Single(user.Subscriptions);
    }

    [Fact]
    public async Task Duplication_of_subscriptions_prevented()
    {
        var (repo, ctx) = CreateDb();
        var userSerivce = new UserService(repo);

        await userSerivce.AddSubscription(TestUser, TestSub);
        await userSerivce.AddSubscription(TestUser, TestSub);

        var user = await repo.GetOrCreate(TestUser);
        Assert.Single(user.Subscriptions);
    }

    [Fact]
    public async Task Adding_new_subscription_dont_remove_other_subscriptions()
    {
        var (repo, ctx) = CreateDb();
        var userSerivce = new UserService(repo);

        await userSerivce.AddSubscription(TestUser, TestSub);
        var someOtherSubscription = new Subscription("other sub", "other pub");
        await userSerivce.AddSubscription(TestUser, someOtherSubscription);

        var user = await repo.GetOrCreate(TestUser);
        Assert.Equal(2, user.Subscriptions.Count);
        Assert.Contains(user.Subscriptions, s => s.SameAs(TestSub));
        Assert.Contains(user.Subscriptions, s => s.SameAs(someOtherSubscription));
    }

    [Fact]
    public async Task Subscription_can_be_removed()
    {
        var (repo, ctx) = CreateDb();
        var userSerivce = new UserService(repo);

        var someOtherSubscription = new Subscription("other sub", "other pub");
        await userSerivce.AddSubscription(TestUser, TestSub);
        await userSerivce.AddSubscription(TestUser, someOtherSubscription);
        await userSerivce.RemoveSubscription(TestUser, someOtherSubscription);

        var user = await repo.GetOrCreate(TestUser);
        Assert.Single(user.Subscriptions);
        Assert.True(user.Subscriptions[0].SameAs(TestSub));
    }

    [Fact]
    public async Task Removing_not_existing_subscription_do_nothing()
    {
        var (repo, ctx) = CreateDb();
        var userSerivce = new UserService(repo);

        var someOtherSubscription = new Subscription("other sub", "other pub");
        await userSerivce.AddSubscription(TestUser, TestSub);
        await userSerivce.RemoveSubscription(TestUser, someOtherSubscription);

        var user = await repo.GetOrCreate(TestUser);
        Assert.Single(user.Subscriptions);
        Assert.True(user.Subscriptions[0].SameAs(TestSub));
    }

    [Fact]
    public async Task Users_can_be_found_for_notification_by_subscription()
    {
        var userService = NewUserService;

        var usersToNotify = await userService
                        .UsersToNotify(TestSub);
        Assert.Empty(usersToNotify);
        const string UIType = "telegram";
        var user1 = new UserModel
        {
            UserId = "user1",
            UIType = UIType,
            Name = "abcd"
        };
        var user2 = new UserModel
        {
            UserId = "user2",
            UIType = UIType,
            Name = "abcd"
        };
        var user3 = new UserModel
        {
            UserId = "user3",
            UIType = UIType,
            Name = "abcd"
        };

        await userService.AddSubscription(user1, TestSub);
        await userService.AddSubscription(user2, TestSub);
        await userService.AddSubscription(user2, new("other source", TestSub.Topic));
        await userService.AddSubscription(user2, new("other source", "other topic"));
        await userService.AddSubscription(user3, new(TestSub.Source, "other topic"));

        usersToNotify = await userService.UsersToNotify(TestSub);
        Assert.Equal(2, usersToNotify.Count);
        Assert.Contains(usersToNotify, u => u.UserId == user1.UserId);
        Assert.Contains(usersToNotify, u => u.UserId == user2.UserId);
    }
}