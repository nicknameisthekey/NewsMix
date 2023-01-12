namespace NewsMix.Abstractions
{
    public interface UserRepository
    {
        Task<List<User>> GetUsers();

        Task UpsertUser(User u);
    }

    public class User
    {
        public string UserId { get; init; } = null!;
        public string UIType { get; init; } = null!;
        public List<UserSubscription> Subscriptions { get; init; } = new();
    }

    public class UserSubscription
    {
        public string FeedName { get; init; } = null!;
        public string PublicationType { get; init; } = null!;
    }
}