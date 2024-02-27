using Microsoft.EntityFrameworkCore;
using NewsMix.Abstractions;
using NewsMix.Models;
using NewsMix.Storage.Entities;

namespace NewsMix.Storage;

public class SqliteRepository(SqliteContext context) : PublicationsRepository, UserRepository
{
    public async Task<bool> IsPublicationNew(string publicationUrl)
    {
        return (await context.FoundPublications.AnyAsync
            (p => p.PublicationUrl == publicationUrl)) == false;
    }

    public async Task CreateNotificationTasks(Publication publication)
    {
        var usersToNotify = await context.Users
            .Include(u => u.Subscriptions)
            .Include(u => u.NotificationTasks)
            .Where(u => u.NotificationTasks.Any(t => t.Url == publication.Url) == false)
            .Where(u => u.Subscriptions.Any(s => s.Source == publication.Source &&
                                                 s.TopicInternalName == publication.TopicInternalName) == true)
            .ToListAsync();

        if (usersToNotify.Any() == false)
            return;

        var hashtag = (await context.NewsTopics
            .SingleAsync(t => t.Enabled && t.NewsSource == publication.Source &&
                              t.InternalName == publication.TopicInternalName)).HashTag;

        foreach (var u in usersToNotify)
        {
            u.NotificationTasks.Add(new NotificationTask()
            {
                Id = Guid.NewGuid().ToString(),
                Url = publication.Url,
                Topic = publication.TopicInternalName,
                CreatedAtUTC = DateTime.UtcNow,
                HashTag = hashtag,
                DoneAtUTC = null
            });
        }

        await context.SaveChangesAsync();
    }

    public async Task<int> NotificationCount()
    {
        return await context.FoundPublications.CountAsync();
    }

    public async Task AddPublication(Publication publication)
    {
        if (await IsPublicationNew(publication.Url) == false)
            return;

        context.FoundPublications.Add(new FoundPublication
        {
            PublicationUrl = publication.Url,
            TopicInternalName = publication.TopicInternalName,
            Source = publication.Source,
            CreatedAtUTC = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }

    public async Task<User> GetOrCreate(UserModel userToFind)
    {
        var result = await context.Users
            .Include(u => u.Subscriptions)
            .FirstOrDefaultAsync(u => u.UIType == userToFind.UIType && u.ExternalUserId == userToFind.ExternalUserId);

        if (result == null)
        {
            result = new User
            {
                ExternalUserId = userToFind.ExternalUserId,
                Name = userToFind.Name,
                UIType = userToFind.UIType,
                CreatedAt = DateTime.UtcNow,
                Subscriptions = []
            };
            context.Users.Add(result);
            await context.SaveChangesAsync();
        }

        await UpdateName(userToFind, result);

        return result;
    }

    public async Task AddSubscription(UserModel user, Subscription sub)
    {
        var u = await GetOrCreate(user);

        if (u.Subscriptions.Any(s => s.SameAs(sub)))
            return; //todo log

        u.Subscriptions.Add(new Subscription
        {
            Source = sub.Source,
            TopicInternalName = sub.TopicInternalName,
            CreatedOnUTC = DateTime.UtcNow,
            InternalUserId = u.Id
        });

        u.UserActions.Add(new UserAction
        {
            ActionType = ActionType.Subscribe,
            Topic = sub.TopicInternalName,
            Source = sub.Source,
            CreatedAtUTC = DateTime.UtcNow,
            InternalUserId = u.Id
        });
        await context.SaveChangesAsync();
    }

    public async Task RemoveSubscription(UserModel user, Subscription sub)
    {
        var u = await GetOrCreate(user);
        var subToRemove = u.Subscriptions.FirstOrDefault(s => s.SameAs(sub));

        u.Subscriptions.Remove(subToRemove!);

        u.UserActions.Add(new UserAction
        {
            ActionType = ActionType.Unsubscribe,
            Topic = sub.TopicInternalName,
            Source = sub.Source,
            CreatedAtUTC = DateTime.UtcNow,
            InternalUserId = u.Id
        });
        await context.SaveChangesAsync();
    }

    public async Task<int> UsersCount()
    {
        return await context.Users.CountAsync();
    }

    public async Task<BotSentMessage?> ActiveKeyboardMessage(string externalUserId)
    {
        return await context.BotSentMessages
            .SingleOrDefaultAsync(m => m.ExternalUserId == externalUserId &&
                              m.MessageType == MessageType.Keyboard &&
                              m.DeletedAtUTC.HasValue == false);
    }

    private async Task UpdateName(UserModel newValue, User oldValue)
    {
        if (newValue.Name != oldValue.Name)
        {
            oldValue.Name = newValue.Name;
            await context.SaveChangesAsync();
        }
    }
}