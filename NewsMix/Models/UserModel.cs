using NewsMix.Storage.Entities;

namespace NewsMix.Models;

public class UserModel
{
    public string ExternalUserId { get; init; } = null!;
    public string UIType { get; init; } = null!;
    public string Name { get; init; } = null!;
    
    public static UserModel FromEntity(User user) => new UserModel
    {
        ExternalUserId = user.ExternalUserId,
        UIType = user.UIType,
        Name = user.Name
    };

    public override string ToString()
    {
        return $"UserId {ExternalUserId}, Name {Name}";
    }
}