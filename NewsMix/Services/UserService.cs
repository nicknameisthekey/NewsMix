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

    public async Task<IReadOnlyCollection<User>> GetUsersToNotifyBy(string feedName, string publicationType)
    {
        var users = await _userRepository.GetUsers();
        var result = users.Where(u => u.Subscriptions
                .Any(s => s.FeedName == feedName && s.PublicationType == publicationType))
                .ToList();
        return result;
    }

    public async Task AddSubscription(string userId, string feedName, string publicationType)
    {
        var users = await _userRepository.GetUsers();
        var user = users.FirstOrDefault(u => u.UserId == userId);
        if (user == null)
            throw new Exception($"user is null by userId {userId}"); //todo: add user instead

        if (user.Subscriptions.Any(s => s.FeedName == feedName
                        && s.PublicationType == publicationType))
            return;

        user.Subscriptions.Add(new UserSubscription
        {
            FeedName = feedName,
            PublicationType = publicationType
        });

        await _userRepository.UpsertUser(user);
    }
    public async Task RemoveSubscription(string userId, string feedName, string publicationType)
    {
        var users = await _userRepository.GetUsers();
        var user = users.FirstOrDefault(u => u.UserId == userId);
        if (user == null)
            throw new Exception($"user is null by userId {userId}"); //todo: add user instead

        var subscription = user.Subscriptions.FirstOrDefault(s => s.FeedName == feedName
                        && s.PublicationType == publicationType);

        if (subscription == null)
            return;

        user.Subscriptions.Remove(subscription);

        await _userRepository.UpsertUser(user);
    }

    public async Task<Dictionary<string, List<string>>> GetUserSubscriptions(string userId)
    {
        var users = await _userRepository.GetUsers();
        var user = users.FirstOrDefault(u => u.UserId == userId);
        if (user == null)
        {
            _logger?.LogWarning("user not found by {userId}", userId);
            return new Dictionary<string, List<string>>();
        }
        var result = user.Subscriptions.GroupBy(s => s.FeedName)
                .ToDictionary(g => g.Key, g => g.Select(s => s.PublicationType).ToList());
        return result;
    }

    public async Task AddUser(string userId, string UIType)
    {
        var users = await _userRepository.GetUsers();
        if (users.Any(u => u.UserId == userId && u.UIType == UIType))
        {
            _logger?.LogWarning("Attempt to add already existing user with {userId} and {UIType}", userId, UIType);
            return;
        }

        await _userRepository.UpsertUser(new User
        {
            UserId = userId,
            UIType = UIType
        });
    }
}