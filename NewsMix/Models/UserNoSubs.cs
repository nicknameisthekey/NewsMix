namespace NewsMix.Models;

public class UserNoSubs //todo rename
{
    public string UserId { get; init; } = null!;
    public string UIType { get; init; } = null!;
    public string Name { get; init; } = null!;

    public override string ToString()
    {
        return $"UserId {UserId}, Name {Name}";
    }
}