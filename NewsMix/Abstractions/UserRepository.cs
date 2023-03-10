using NewsMix.Models;
using NewsMix.Storage.Entites;

namespace NewsMix.Abstractions;

public interface UserRepository
{
    Task<List<UserModel>> GetToNotify(Subscription sub);

    Task<NewsMix.Storage.Entites.User> GetOrCreate(UserModel user);

    Task AddSubscription(UserModel user, Subscription sub);

    Task RemoveSubscription(UserModel user, Subscription sub);

    Task<int> UsersCount();
}
