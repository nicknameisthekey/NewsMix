using NewsMix.Models;

namespace NewsMix.Abstractions;

public interface UserRepository
{
    Task<List<User>> GetUsers();

    Task UpsertUser(User u);
}