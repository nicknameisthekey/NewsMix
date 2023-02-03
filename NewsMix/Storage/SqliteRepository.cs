using Microsoft.EntityFrameworkCore;
using NewsMix.Abstractions;
using NewsMix.Models;
using NewsMix.Storage.Entites;

public class SqliteRepository : PublicationRepository, UserRepository
{
    private readonly SqliteContext _context;

    public SqliteRepository(SqliteContext context)
    {
        _context = context;
    }

    public async Task<bool> IsPublicationNew(string publicationUniqeID)
    {
        return (await _context.NotifiedPublications.AnyAsync
            (p => p.PublicationUniqeID == publicationUniqeID)) == false;
    }

    public async Task<int> NotificationCount()
    {
        return await _context.NotifiedPublications.CountAsync();
    }

    public async Task SetPublicationNotified(string publicationUniqeID)
    {
        if (await IsPublicationNew(publicationUniqeID) == false)
            return;

        _context.NotifiedPublications.Add(new NotifiedPublication
        {
            PublicationUniqeID = publicationUniqeID,
            CreatedAt = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserModel>> GetToNotify(Subscription sub)
    {
        return (await _context.Users
        .Include(u => u.Subscriptions)
        .ToListAsync()) //deliberately
        .Where(u => u.Subscriptions.Any(s => s.SameAs(sub)))
        .Select(UserModel.FromEntity).ToList();
    }

    public async Task<User> GetOrCreate(UserModel userToFind)
    {
        var result = await _context.Users
            .Include(u => u.Subscriptions)
            .FirstOrDefaultAsync(u => u.UIType == userToFind.UIType && u.UserId == userToFind.UserId);

        if (result == null)
        {
            result = new User
            {
                UserId = userToFind.UserId,
                Name = userToFind.Name,
                UIType = userToFind.UIType,
                CreatedAt = DateTime.Now,
                Subscriptions = new List<Subscription>()
            };
            _context.Users.Add(result);
            await _context.SaveChangesAsync();
        }

        await UpdateName(userToFind, result);

        return result;
    }

    public async Task AddSubscription(UserModel user, Subscription sub)
    {
        var u = await GetOrCreate(user);

        if (u.Subscriptions.Any(s => s.Source == sub.Source && s.Topic == sub.Topic))
            return;

        u.Subscriptions.Add(new Subscription
        {
            Source = sub.Source,
            Topic = sub.Topic
        });

        u.UserActions.Add(new UserAction
        {
            ActionType = ActionType.Subscribe,
            Topic = sub.Topic,
            Source = sub.Source,
            CreatedAt = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }

    public async Task RemoveSubscription(UserModel user, Subscription sub)
    {
        var u = await GetOrCreate(user);
        var subToRemove = u.Subscriptions.FirstOrDefault
            (s => s.Source == sub.Source && s.Topic == sub.Topic);

        u.Subscriptions.Remove(subToRemove!);

        u.UserActions.Add(new UserAction
        {
            ActionType = ActionType.Unsubscribe,
            Topic = sub.Topic,
            Source = sub.Source,
            CreatedAt = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }

    public async Task<int> UsersCount()
    {
        return await _context.Users.CountAsync();
    }

    private async Task UpdateName(UserModel newValue, User oldValue)
    {
        if (newValue.Name != oldValue.Name)
        {
            oldValue.Name = newValue.Name;
            await _context.SaveChangesAsync();
        }
    }
}