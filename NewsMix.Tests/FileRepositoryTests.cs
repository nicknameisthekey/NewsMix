using NewsMix.Abstractions;
using NewsMix.Storage;
using static TestHelpers;

namespace NewsMix.Tests;

public class FileRepositoryTests : IDisposable
{
    FileRepository NewFileRepository => new FileRepository(MockIConfiguration);
    [Fact]
    public void Not_existing_required_files_created_in_ctor()
    {
        var repo = NewFileRepository;
        Assert.True(File.Exists(repo._publicationNotifiedListTxtFile));
        Assert.True(File.Exists(repo._usersJsonFile));
    }

    [Fact]
    public async Task AddToPublicationNotifiedList_doesnt_add_duplicates()
    {
        var repo = NewFileRepository;
        File.Create(repo._publicationNotifiedListTxtFile).Close();

        const string someValue = "this is value";
        await repo.AddToPublicationNotifiedList(someValue);
        await repo.AddToPublicationNotifiedList(someValue);

        var fileLines = File.ReadAllLines(repo._publicationNotifiedListTxtFile);
        Assert.Single(fileLines);
    }

    [Fact]
    public async Task IsPublicationNew_returns_true_if_publication_is_not_in_notified_list()
    {
        var repo = NewFileRepository;

        const string someValue = "this is value";
        var result = await repo.IsPublicationNew(someValue);
        Assert.True(result);

        await repo.AddToPublicationNotifiedList(someValue);
        result = await repo.IsPublicationNew(someValue);
        Assert.False(result);
    }

    [Fact]
    public async Task UpserUser_adds_new_user_if_none_present_with_userId()
    {
        var repo = NewFileRepository;
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
    }

    [Fact]
    public async Task UpserUser_updates_new_user_if_user_with_userId_exists()
    {
        var repo = NewFileRepository;

        User user = new Abstractions.User
        {
            UserId = "1234",
            UIType = "Telegram",
            Subscriptions = new List<Subscription> { new("feed1", "pub1") }
        };
        await repo.UpsertUser(user);
        var users = await repo.GetUsers();
        Assert.Single(users);
        Assert.Single(users[0].Subscriptions);
        Assert.Equal(users[0].Subscriptions[0].FeedName, user.Subscriptions[0].FeedName);
        Assert.Equal(users[0].Subscriptions[0].PublicationType, user.Subscriptions[0].PublicationType);

        user.Subscriptions[0] = new("feed2", "pub2");

        await repo.UpsertUser(user);
        users = await repo.GetUsers();
        Assert.Single(users);
        Assert.Single(users[0].Subscriptions);
        Assert.Equal(users[0].Subscriptions[0].FeedName, user.Subscriptions[0].FeedName);
        Assert.Equal(users[0].Subscriptions[0].PublicationType, user.Subscriptions[0].PublicationType);
    }

    public void Dispose()
    {
        EmptyTestFilesDirectory();
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