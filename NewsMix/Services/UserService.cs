using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Storage.Entites;
using NewsMix.Models;

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

    public async Task<IReadOnlyCollection<UserModel>> UsersToNotify(Subscription sub)
    {
        return await _userRepository.GetToNotify(sub);
    }

    public async Task AddSubscription(UserModel user, Subscription sub)
    {
        await _userRepository.AddSubscription(user, sub);
    }

    public async Task RemoveSubscription(UserModel user, Subscription sub)
    {
        await _userRepository.RemoveSubscription(user, sub);
    }

    public async Task<List<Subscription>> Subscriptions(UserModel user, string? source = null)
    {
        var u = await _userRepository.GetOrCreate(user);
        return source switch
        {
            null => u.Subscriptions,
            not null => u.Subscriptions.Where(s => s.Source == source).ToList()
        };
    }
}