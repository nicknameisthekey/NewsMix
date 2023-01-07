using NewsMix.DAL.Entities;
using NewsMix.DAL.Repositories.Abstraction;

namespace NewsMix.Core.Services;
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

        user.Subscriptions.Add(new UserSubscription
        {
            FeedName = feedName,
            PublicationType = publicationType
        });

        await _userRepository.UpsertUser(user);
    }
}