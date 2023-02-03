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

    public Task<List<UserNoSubs>> GetToNotify(Subscription sub)
    {
        return _context.Users
        .Include(u => u.Subscriptions)
        .Where(u => u.Subscriptions.Any(s => s.Source == sub.Source && s.Topic == sub.Topic))
        .Select(u => new UserNoSubs
        {
            UserId = u.UserId,
            UIType = u.UIType,
            Name = u.Name
        }).ToListAsync();
    }

    public async Task<User> GetOrCreate(UserNoSubs user)
    {
        var result = await _context.Users
            .Include(u => u.Subscriptions)
            .FirstOrDefaultAsync
            (u => u.UIType == user.UIType && u.UserId == user.UserId);

        if (result == null)
        {
            result = new User
            {
                UserId = user.UserId,
                Name = user.Name,
                UIType = user.UIType,
                CreatedAt = DateTime.Now,
                Subscriptions = new List<Subscription>()
            };
            _context.Users.Add(result);
            await _context.SaveChangesAsync();
        }

        return result;
    }

    public async Task AddSubscription(UserNoSubs user, Subscription sub)
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

    public async Task RemoveSubscription(UserNoSubs user, Subscription sub)
    {
        var u = await GetOrCreate(user);
        var subToRemove = u.Subscriptions.FirstOrDefault
            (s => s.Source == sub.Source && s.Topic == sub.Topic);

        u.Subscriptions.Remove(subToRemove); //todo

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
}