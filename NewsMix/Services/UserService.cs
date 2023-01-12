using NewsMix.Abstractions;

namespace NewsMix.Services;
public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
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
            throw new Exception($"user is null by userId {userId}");

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

    public async Task AddUser(string userId, string UIType)
    {
        var users = await _userRepository.GetUsers();
        if (users.Any(u => u.UserId == userId))
            throw new Exception("user already exists");

        await _userRepository.UpsertUser(new User
        {
            UserId = userId,
            UIType = UIType
        });
    }
}