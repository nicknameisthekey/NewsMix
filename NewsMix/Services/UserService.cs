using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Storage.Entites;
using NewsMix.Models;

namespace NewsMix.Services;

public class UserService(UserRepository userRepository, ILogger<UserService>? logger = null)
{
    private readonly ILogger<UserService>? _logger = logger;

    public async Task<IReadOnlyCollection<UserModel>> UsersToNotify(Subscription sub)
    {
        return await userRepository.GetToNotify(sub);
    }

    public async Task AddSubscription(UserModel user, Subscription sub)
    {
        await userRepository.AddSubscription(user, sub);
    }

    public async Task RemoveSubscription(UserModel user, Subscription sub)
    {
        await userRepository.RemoveSubscription(user, sub);
    }

    public async Task<List<Subscription>> Subscriptions(UserModel user, string source)
    {
        var u = await userRepository.GetOrCreate(user);
        return u.Subscriptions.Where(s => s.Source == source).ToList();
    }
}