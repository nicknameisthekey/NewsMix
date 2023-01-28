namespace NewsMix.Models;

public class User
{
    public string UserId { get; init; } = null!;
    public string UIType { get; init; } = null!;
    public List<Subscription> Subscriptions { get; init; } = new();
}