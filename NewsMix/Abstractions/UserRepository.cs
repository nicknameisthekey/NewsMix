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
        public List<Subscription> Subscriptions { get; init; } = new();
    }

#pragma warning disable CS0659 //no GetHashcode()
    public class Subscription
    {
        public string FeedName { get; init; } = null!;
        public string PublicationType { get; init; } = null!;

        public Subscription(string feedName, string publicationType)
        {
            FeedName = feedName;
            PublicationType = publicationType;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Subscription sub)
            {
                return sub.FeedName == FeedName && sub.PublicationType == PublicationType;
            }

            return base.Equals(obj);
        }

        public override string ToString() => $"feed: {FeedName}, pubType: {PublicationType}";
    }
}