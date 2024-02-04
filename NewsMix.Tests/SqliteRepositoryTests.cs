using NewsMix.Models;
using NewsMix.Storage;
using NewsMix.Storage.Entities;
using static NewsMix.Tests.TestHelpers;

namespace NewsMix.Tests;

public class SqliteRepositoryTests
{
    private readonly SqliteContext ctx;
    private readonly SqliteRepository repo;
    public SqliteRepositoryTests() => (repo, ctx) = CreateDb();
    public Publication Publication =>  new Publication()
    {
        Url = "https://123.com",
        TopicInternalName = "abcd",
        HashTag = null,
        Source = "source",
    };

    [Fact]
    public async Task SetPublicationNotified_doesnt_add_duplicates()
    {
        await repo.AddPublication(Publication);
        await repo.AddPublication(Publication);

        Assert.Single(ctx.FoundPublications.ToList());
    }

    [Fact]
    public async Task Publication_is_new_if_not_in_notified_list()
    {
        Assert.True(await repo.IsPublicationNew(Publication.Url));
        await repo.AddPublication(Publication);
        Assert.False(await repo.IsPublicationNew(Publication.Url));
    }

    [Fact]
    public async Task Count_notifications_shows_all_notified_publications()
    {
        Assert.Equal(0, await repo.NotificationCount());

        var pub = new Publication()
        {
            Url = "https://123.com",
            TopicInternalName = "abcd",
            HashTag = null,
            Source = "source",
        };
        await repo.AddPublication(pub);
        Assert.Equal(1, await repo.NotificationCount());
        
        var pub2 = new Publication()
        {
            Url = "https://321.com",
            TopicInternalName = "abcd",
            HashTag = null,
            Source = "source",
        };
        await repo.AddPublication(pub2);
        Assert.Equal(2, await repo.NotificationCount());
    }

    [Fact]
    public async Task GetOrCreate_adds_new_user_if_none_present_by_user_id()
    {
        const string u1Id = "1234";
        const string u2Id = "4321";

        await repo.GetOrCreate(new UserModel
        {
            ExternalUserId = u1Id,
            UIType = "Telegram",
            Name = u1Id
        });

        await repo.GetOrCreate(new UserModel
        {
            ExternalUserId = u2Id,
            UIType = "Telegram",
            Name = u2Id
        });

        var users = ctx.Users.ToList();
        Assert.Equal(2, users.Count);
        Assert.Contains(users, u => u.ExternalUserId == u1Id);
        Assert.Contains(users, u => u.ExternalUserId == u2Id);
    }

    [Fact]
    public async Task GetOrCreate_returns_existing_user()
    {
        var user = new User
        {
            ExternalUserId = "1234",
            UIType = "telegram",
            Name = "1234"
        };
        ctx.Users.Add(user);
        ctx.SaveChanges();

        var result = await repo.GetOrCreate(new UserModel
        {
            ExternalUserId = user.ExternalUserId,
            UIType = user.UIType,
            Name = user.Name
        });

        Assert.Single(ctx.Users.ToArray());
        Assert.Equal(user.Id, result.Id);
    }
}