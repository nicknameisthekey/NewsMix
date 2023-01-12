using NewsMix.Abstractions;
using NewsMix.Storage;
using static TestHelpers;

namespace NewsMix.Tests;

public class UnitTest1
{
    [Fact]
    public void Not_existing_required_files_created_in_ctor()
    {
        var repo = new FileRepository(MockIConfiguration);
        Assert.True(File.Exists(repo._publicationNotifiedListTxtFile));
        Assert.True(File.Exists(repo._usersJsonFile));
    }

    [Fact]
    public async Task AddToPublicationNotifiedList_doesnt_add_duplicates()
    {
        PrepareFileStorage();
        var repo = new FileRepository(MockIConfiguration);
        File.Create(repo._publicationNotifiedListTxtFile).Close();

        const string someValue = "this is value";
        await repo.AddToPublicationNotifiedList(someValue);
        await repo.AddToPublicationNotifiedList(someValue);

        var fileLines = File.ReadAllLines(repo._publicationNotifiedListTxtFile);
        Assert.Single(fileLines);

        File.Delete(repo._publicationNotifiedListTxtFile);
    }

    [Fact]
    public async Task IsPublicationNew_returns_true_if_publication_is_not_in_notified_list()
    {
        PrepareFileStorage();
        var repo = new FileRepository(MockIConfiguration);
        File.Create(repo._publicationNotifiedListTxtFile).Close();

        const string someValue = "this is value";
        var result = await repo.IsPublicationNew(someValue);
        Assert.True(result);

        await repo.AddToPublicationNotifiedList(someValue);
        result = await repo.IsPublicationNew(someValue);
        Assert.False(result);

        File.Delete(repo._publicationNotifiedListTxtFile);
    }

    [Fact]
    public async Task UpserUser_adds_new_user_if_none_present_with_userId()
    {
        PrepareFileStorage();
        var repo = new FileRepository(MockIConfiguration);
        File.Create(repo._usersJsonFile).Close();
        const string u1Id = "1234";
        const string u2Id = "4321";

        await repo.UpsertUser(new Abstractions.User
        {
            UserId = u1Id,
            UIType = "Telegram",
        });

        await repo.UpsertUser(new Abstractions.User
        {
            UserId = u2Id,
            UIType = "Telegram",
        });

        var users = await repo.GetUsers();
        Assert.Equal(2, users.Count);

        users.UserShouldBeIncollection(u1Id);
        users.UserShouldBeIncollection(u1Id);

        File.Delete(repo._usersJsonFile);
    }

    [Fact]
    public async Task UpserUser_updates_new_user_if_user_with_userId_exists()
    {
        PrepareFileStorage();
        var repo = new FileRepository(MockIConfiguration);
        File.Create(repo._usersJsonFile).Close();

        User user = new Abstractions.User
        {
            UserId = "1234",
            UIType = "Telegram",
            Subscriptions = new List<UserSubscription>{
                new UserSubscription{
                     FeedName = "feed1",
                     PublicationType = "pub1"
                }
            }
        };
        await repo.UpsertUser(user);
        var users = await repo.GetUsers();
        Assert.Single(users);
        Assert.Single(users[0].Subscriptions);
        Assert.Equal(users[0].Subscriptions[0].FeedName, user.Subscriptions[0].FeedName);
        Assert.Equal(users[0].Subscriptions[0].PublicationType, user.Subscriptions[0].PublicationType);

        user.Subscriptions[0] = new UserSubscription { FeedName = "feed2", PublicationType = "pub2" };

        await repo.UpsertUser(user);
        users = await repo.GetUsers();
        Assert.Single(users);
        Assert.Single(users[0].Subscriptions);
        Assert.Equal(users[0].Subscriptions[0].FeedName, user.Subscriptions[0].FeedName);
        Assert.Equal(users[0].Subscriptions[0].PublicationType, user.Subscriptions[0].PublicationType);

        File.Delete(repo._usersJsonFile);
    }
}

public static partial class TestHelpers
{
    public static void UserShouldBeIncollection(this List<User> users, string userId)
    {
        var u = users.FirstOrDefault(u => u.UserId == userId);
        Assert.NotNull(u);
    }
}