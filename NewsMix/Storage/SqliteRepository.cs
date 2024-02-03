using Microsoft.EntityFrameworkCore;
using NewsMix.Abstractions;
using NewsMix.Models;
using NewsMix.Storage.Entities;

namespace NewsMix.Storage;

public class SqliteRepository(SqliteContext context) : PublicationsRepository, UserRepository
{
    public async Task<bool> IsPublicationNew(string publicationUniqueID)
    {
        return (await context.NotifiedPublications.AnyAsync
            (p => p.PublicationUniqeID == publicationUniqueID)) == false;
    }

    public async Task<int> NotificationCount()
    {
        return await context.NotifiedPublications.CountAsync();
    }

    public async Task SetPublicationNotified(string publicationUniqueID)
    {
        if (await IsPublicationNew(publicationUniqueID) == false)
            return;

        context.NotifiedPublications.Add(new NotifiedPublication
        {
            PublicationUniqeID = publicationUniqueID,
            CreatedAt = DateTime.Now
        });
        await context.SaveChangesAsync();
    }

    public async Task<List<UserModel>> GetToNotify(Subscription sub)
    {
        return (await context.Users
                .Include(u => u.Subscriptions)
                .ToListAsync()) //deliberately
            .Where(u => u.Subscriptions.Any(s => s.SameAs(sub)))
            .Select(UserModel.FromEntity).ToList();
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

    private async Task UpdateName(UserModel newValue, User oldValue)
    {
        if (newValue.Name != oldValue.Name)
        {
            oldValue.Name = newValue.Name;
            await context.SaveChangesAsync();
        }
    }
}