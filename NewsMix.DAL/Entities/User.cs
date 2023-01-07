namespace NewsMix.DAL.Entities;
public class User
{
    public string UserId { get; init; } = null!;
    public string UIType { get; init; } = null!;
    public List<UserSubscription> Subscriptions { get; init; } = new();
}