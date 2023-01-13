using NewsMix.Abstractions;
using NewsMix.Services;
using NewsMix.Storage;
using static TestHelpers;

public class UserServiceTests : IDisposable
{
    #region  Default values and helpers
    const string SomeUserID = "12345";
    const string SomeUIType = "telegram";
    Subscription SomeSubscription => new Subscription("feed", "pub");
    UserService NewUserService => new UserService(new FileRepository(MockIConfiguration));
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
    public async Task Adding_subscription_to_non_existing_user_creates_new_user()
    {
        var userSerivce = NewUserService;

        await userSerivce.AddSubscription(SomeUserID, SomeUIType, SomeSubscription);
        await userSerivce.RemoveSubscription(SomeUserID, SomeSubscription);

        var user = await userSerivce.GetOrCreate(SomeUserID, SomeUIType);
        Assert.NotNull(user);
        Assert.Empty(user.Subscriptions);
    }

    [Fact]
    public async Task Duplication_of_subscriptions_prevented()
    {
        var userSerivce = NewUserService;

        await userSerivce.GetOrCreate(SomeUserID, SomeUIType);
        await userSerivce.AddSubscription(SomeUserID, SomeUIType, SomeSubscription);
        await userSerivce.AddSubscription(SomeUserID, SomeUIType, SomeSubscription);

        var user = await userSerivce.GetOrCreate(SomeUserID, SomeUIType);
        Assert.NotNull(user);
        Assert.Single(user.Subscriptions);
    }

    [Fact]
    public async Task Subscription_can_be_removed()
    {
        var userSerivce = NewUserService;

        await userSerivce.AddSubscription(SomeUserID, SomeUIType, SomeSubscription);
        await userSerivce.RemoveSubscription(SomeUserID, SomeSubscription);

        var user = await userSerivce.GetOrCreate(SomeUserID, SomeUIType);
        Assert.NotNull(user);
        Assert.Empty(user.Subscriptions);
    }

    [Fact]
    public async Task Removing_not_existing_subscription_do_nothing()
    {
        var userSerivce = NewUserService;

        await userSerivce.GetOrCreate(SomeUserID, SomeUIType);
        await userSerivce.RemoveSubscription(SomeUserID, SomeSubscription);

        var user = await userSerivce.GetOrCreate(SomeUserID, SomeUIType);
        Assert.NotNull(user);
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
                        .GetUsersToNotifyBy(SomeSubscription);
        Assert.Empty(usersToNotify);
        const string user1 = "user1";
        const string user2 = "user2";
        const string user3 = "user3";

        await userService.AddSubscription(user1, SomeUIType, SomeSubscription);
        await userService.AddSubscription(user2, SomeUIType, SomeSubscription);
        await userService.AddSubscription(user2, SomeUIType, new("other feed", SomeSubscription.PublicationType));
        await userService.AddSubscription(user3, SomeUIType, new("other feed", "other sub"));
        await userService.AddSubscription(user3, SomeUIType, new(SomeSubscription.FeedName, "other sub"));

        usersToNotify = await userService
               .GetUsersToNotifyBy(SomeSubscription);
        Assert.Equal(2, usersToNotify.Count);
        Assert.Contains(usersToNotify, u => u.UserId == user1);
        Assert.Contains(usersToNotify, u => u.UserId == user2);
    }

    public void Dispose()
    {
        EmptyTestFilesDirectory();
    }
}