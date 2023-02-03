using NewsMix.Models;
using NewsMix.Storage.Entites;

namespace NewsMix.Abstractions;

public interface UserRepository
{
    Task<List<UserNoSubs>> GetToNotify(Subscription sub);

    Task<NewsMix.Storage.Entites.User> GetOrCreate(UserNoSubs user);

    Task AddSubscription(UserNoSubs user, Subscription sub);

    Task RemoveSubscription(UserNoSubs user, Subscription sub);

    Task<int> UsersCount();
}
