using NewsMix.Storage.Entities;

namespace NewsMix.Models;

public class UserModel
{
    public string UserId { get; init; } = null!;
    public string UIType { get; init; } = null!;
    public string Name { get; init; } = null!;

    public bool SameAs(UserModel other) => other.UserId == UserId && other.UIType == UIType;

    public static UserModel FromEntity(User user) => new UserModel
    {
        UserId = user.UserId,
        UIType = user.UIType,
        Name = user.Name
    };

    public override string ToString()
    {
        return $"UserId {UserId}, Name {Name}";
    }
}