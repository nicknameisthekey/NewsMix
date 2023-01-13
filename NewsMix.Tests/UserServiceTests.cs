using NewsMix.Abstractions;
using NewsMix.Services;
using NewsMix.Storage;
using static TestHelpers;

public class UserServiceTests
{
    const string SomeUserID = "12345";
    const string SomeUIType = "telegram";
    Subscription SomeSubscription => new Subscription("feed", "pub");
    UserService NewUserService => new UserService(new FileRepository(MockIConfiguration));

    [Fact]
    public async Task Adding_subscription_to_non_existing_user_creates_new_user()
    {
        PrepareFileStorage();
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
        PrepareFileStorage();
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
        PrepareFileStorage();
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
        PrepareFileStorage();
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
        PrepareFileStorage();
        var userSerivce = NewUserService;

        var exception = await Assert.ThrowsAnyAsync<Exception>(
            () => userSerivce.RemoveSubscription(SomeUserID, SomeSubscription));
        Assert.Contains("user is null by userId", exception.Message);
    }

    [Fact]
    public async Task Users_can_be_found_by_feed_and_publication_type()
    {
        PrepareFileStorage();
        var userSerivce = NewUserService;
        const string userId1 = "1234";
        const string userId2 = "4321";
        const string userId3 = "1111";
        const string feedName1 = "feed1";
        const string feedName2 = "feed2";
        const string pubType1 = "pub1";
        const string pubType2 = "pub2";

        await userSerivce.GetOrCreate(userId1, SomeUIType);
        await userSerivce.GetOrCreate(userId2, SomeUIType);
        await userSerivce.GetOrCreate(userId3, SomeUIType);
        await userSerivce.AddSubscription(userId1, SomeUIType, new(feedName1, pubType1));
        await userSerivce.AddSubscription(userId2, SomeUIType, new(feedName1, pubType1));
        await userSerivce.AddSubscription(userId3, SomeUIType, new(feedName1, pubType1));
        await userSerivce.AddSubscription(userId2, SomeUIType, new(feedName2, pubType2));
        await userSerivce.AddSubscription(userId3, SomeUIType, new(feedName2, pubType2));
        await userSerivce.AddSubscription(userId3, SomeUIType, new("random", "random_random"));

        var users_f1_p1 = await userSerivce.GetUsersToNotifyBy(feedName1, pubType1);
        Assert.Contains(users_f1_p1, u => u.UserId == userId1);
        Assert.Contains(users_f1_p1, u => u.UserId == userId2);
        Assert.Contains(users_f1_p1, u => u.UserId == userId3);
        Assert.Equal(3, users_f1_p1.Count);
        var users_f2_p2 = await userSerivce.GetUsersToNotifyBy(feedName2, pubType2);
        Assert.Contains(users_f2_p2, u => u.UserId == userId3);
        Assert.Contains(users_f2_p2, u => u.UserId == userId2);
        Assert.Equal(2, users_f2_p2.Count);
        var users_f1_p2 = await userSerivce.GetUsersToNotifyBy(feedName1, pubType2);
        Assert.Empty(users_f1_p2);
    }
}