using NewsMix.Abstractions;
using NewsMix.Models;
using NewsMix.Storage;
using static TestHelpers;

public class UserServiceTests : IDisposable
{
    #region  Default values and helpers
    const string SomeUserID = "12345";
    const string SomeUIType = "telegram";
    Subscription SomeSubscription => new Subscription("source", "topic");

    #endregion

    [Fact]
    public async Task User_created_if_not_found()
    {
        var userService = NewUserService;
        var user = await userService.GetOrCreate(SomeUserID, SomeUIType);
        var fileRepo = new FileRepository(MockIConfiguration);
        var users = await fileRepo.GetUsers();
        Assert.Single(users);
        Assert.Equal(SomeUserID, users[0].UserId);
        Assert.Equal(SomeUIType, users[0].UIType);
    }

    [Fact]
    public async Task User_can_be_received_if_found()
    {
        var fileRepo = new FileRepository(MockIConfiguration);
        await fileRepo.UpsertUser(new User
        {
            UserId = SomeUserID,
            UIType = SomeUIType,
            Subscriptions = new() { SomeSubscription }
        });

        var userService = NewUserService;
        var user = await userService.GetOrCreate(SomeUserID, SomeUIType);
        Assert.Equal(SomeUserID, user.UserId);
        Assert.Equal(SomeUIType, user.UIType);
        Assert.Single(user.Subscriptions);
        Assert.Contains(SomeSubscription, user.Subscriptions);
    }

    [Fact]
    public async Task Adding_subscription_to_non_existing_user_creates_new_user_with_subscription()
    {
        var userSerivce = NewUserService;

        await userSerivce.AddSubscription(SomeUserID, SomeUIType, SomeSubscription);

        var user = await userSerivce.GetOrCreate(SomeUserID, SomeUIType);
        Assert.Single(user.Subscriptions);
    }

    [Fact]
    public async Task Duplication_of_subscriptions_prevented()
    {
        var userSerivce = NewUserService;

        await userSerivce.AddSubscription(SomeUserID, SomeUIType, SomeSubscription);
        await userSerivce.AddSubscription(SomeUserID, SomeUIType, SomeSubscription);

        var user = await userSerivce.GetOrCreate(SomeUserID, SomeUIType);
        Assert.Single(user.Subscriptions);
    }

    [Fact]
    public async Task Adding_new_subscription_dont_remove_other_subscriptions()
    {
        var userService = NewUserService;

        var someOtherSubscription = new Subscription("other sub", "other pub");
        await userService.AddSubscription(SomeUserID, SomeUIType, SomeSubscription);
        await userService.AddSubscription(SomeUserID, SomeUIType, someOtherSubscription);

        var user = await userService.GetOrCreate(SomeUserID, SomeUIType);
        Assert.Equal(2, user.Subscriptions.Count);
        Assert.Contains(SomeSubscription, user.Subscriptions);
        Assert.Contains(someOtherSubscription, user.Subscriptions);
    }

    [Fact]
    public async Task Subscription_can_be_removed()
    {
        var userSerivce = NewUserService;

        var someOtherSubscription = new Subscription("other sub", "other pub");
        await userSerivce.AddSubscription(SomeUserID, SomeUIType, SomeSubscription);
        await userSerivce.AddSubscription(SomeUserID, SomeUIType, someOtherSubscription);
        await userSerivce.RemoveSubscription(SomeUserID, someOtherSubscription);

        var user = await userSerivce.GetOrCreate(SomeUserID, SomeUIType);
        Assert.Single(user.Subscriptions);
        Assert.Equal(SomeSubscription, user.Subscriptions[0]);
    }

    [Fact]
    public async Task Removing_not_existing_subscription_do_nothing()
    {
        var userSerivce = NewUserService;

        await userSerivce.GetOrCreate(SomeUserID, SomeUIType);
        await userSerivce.RemoveSubscription(SomeUserID, SomeSubscription);

        var user = await userSerivce.GetOrCreate(SomeUserID, SomeUIType);
        Assert.Empty(user.Subscriptions);
    }

    [Fact]
    public async Task Removing_subscription_from_non_existing_user_throws()
    {
        var userSerivce = NewUserService;

        var exception = await Assert.ThrowsAnyAsync<Exception>(
            () => userSerivce.RemoveSubscription(SomeUserID, SomeSubscription));
        Assert.Contains("user is null by userId", exception.Message);
    }

    [Fact]
    public async Task Users_can_be_found_for_notification_by_subscription()
    {
        var userService = NewUserService;

        var usersToNotify = await userService
                        .UsersToNotify(SomeSubscription);
        Assert.Empty(usersToNotify);
        const string user1 = "user1";
        const string user2 = "user2";
        const string user3 = "user3";

        await userService.AddSubscription(user1, SomeUIType, SomeSubscription);
        await userService.AddSubscription(user2, SomeUIType, SomeSubscription);
        await userService.AddSubscription(user2, SomeUIType, new("other source", SomeSubscription.Topic));
        await userService.AddSubscription(user3, SomeUIType, new("other source", "other topic"));
        await userService.AddSubscription(user3, SomeUIType, new(SomeSubscription.Source, "other topic"));

        usersToNotify = await userService
               .UsersToNotify(SomeSubscription);
        Assert.Equal(2, usersToNotify.Count);
        Assert.Contains(usersToNotify, u => u.UserId == user1);
        Assert.Contains(usersToNotify, u => u.UserId == user2);
    }

    public void Dispose()
    {
        EmptyTestFilesDirectory();
    }
}