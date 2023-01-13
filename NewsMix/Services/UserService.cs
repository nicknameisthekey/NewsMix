using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;

namespace NewsMix.Services;
public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly ILogger<UserService>? _logger;
    public UserService(UserRepository userRepository, ILogger<UserService>? logger = null)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<User>> GetUsersToNotifyBy(Subscription sub)
    {
        var users = await _userRepository.GetUsers();
        return users.Where(u => u.Subscriptions.Contains(sub)).ToList();
    }

    public async Task AddSubscription(string userId, string UIType, Subscription sub)
    {
        var user = await GetOrCreate(userId, UIType);

        if (user.Subscriptions.Contains(sub))
            return;

        user.Subscriptions.Add(sub);
        await _userRepository.UpsertUser(user);
    }

    public async Task RemoveSubscription(string userId, Subscription sub)
    {
        var user = await GetUser(userId, throwIfNone: true);

        user!.Subscriptions.Remove(sub);
        await _userRepository.UpsertUser(user);
    }

    public async Task<Dictionary<string, List<string>>> GetUserSubscriptions(string userId) //todo refactor for <(list<sub> subbed and not subbed)>
    {
        var user = await GetUser(userId);
        return user switch
        {
            null => new(),
            not null => user.Subscriptions.GroupBy(s => s.FeedName)
                .ToDictionary(g => g.Key, g => g.Select(s => s.PublicationType).ToList())
        };
    }

    public async Task<User> GetOrCreate(string userId, string UIType)
    {
        var user = await GetUser(userId, throwIfNone: false);
        return user switch
        {
            null => await CreateUser(userId, UIType),
            not null => user
        };
    }

    private async Task<User?> GetUser(string userId, bool throwIfNone = false)
    {
        var users = await _userRepository.GetUsers();
        var user = users.SingleOrDefault(u => u.UserId == userId);
        if (throwIfNone && user == null)
            throw new Exception($"user is null by userId {userId}");
        return user;
    }

    private async Task<User> CreateUser(string userId, string UIType)
    {
        var newUser = new User { UserId = userId, UIType = UIType };
        await _userRepository.UpsertUser(newUser);
        return newUser;
    }
}