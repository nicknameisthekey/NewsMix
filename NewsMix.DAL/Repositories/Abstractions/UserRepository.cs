using NewsMix.DAL.Entities;

namespace NewsMix.DAL.Repositories.Abstraction;

public interface UserRepository
{
    Task<List<User>> GetUsers();

    Task UpsertUser(User u);
}