using NewsMix.Services;
using NewsMix.Storage;
using static TestHelpers;

public class UserServiceTests
{
    [Fact]
    public async Task Duplication_of_subscriptions_prevented()
    {
        PrepareFileStorage();
        var fileRepo = new FileRepository(MockIConfiguration);
        var userSerivce = new UserService(fileRepo);
        const string userId = "1234";
        const string feedName = "feed";
        const string pubType = "pub";

        await userSerivce.AddUser(userId, "telegram");
        await userSerivce.AddSubscription(userId, feedName, pubType);
        await userSerivce.AddSubscription(userId, feedName, pubType);
        await userSerivce.AddSubscription(userId, feedName, pubType);
        await userSerivce.AddSubscription(userId, feedName, pubType);

        var users = await fileRepo.GetUsers();
        Assert.Single(users);
    }

    [Fact]
    public async Task Users_can_be_found_by_feed_and_publication_type()
    {
        PrepareFileStorage();
        var userSerivce = new UserService(new FileRepository(MockIConfiguration));
        const string userId1 = "1234";
        const string userId2 = "4321";
        const string userId3 = "1111";
        const string feedName1 = "feed1";
        const string feedName2 = "feed2";
        const string pubType1 = "pub1";
        const string pubType2 = "pub2";

        await userSerivce.AddUser(userId1, "telegram");
        await userSerivce.AddUser(userId2, "telegram");
        await userSerivce.AddUser(userId3, "telegram");
        await userSerivce.AddSubscription(userId1, feedName1, pubType1);
        await userSerivce.AddSubscription(userId2, feedName1, pubType1);
        await userSerivce.AddSubscription(userId3, feedName1, pubType1);
        await userSerivce.AddSubscription(userId2, feedName2, pubType2);
        await userSerivce.AddSubscription(userId3, feedName2, pubType2);
        await userSerivce.AddSubscription(userId3, "random", "random_random");

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