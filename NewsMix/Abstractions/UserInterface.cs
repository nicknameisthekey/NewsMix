namespace NewsMix.Abstractions;

public interface UserInterface
{
    string UIName { get; }
    Task NotifyUser(string externalUserId, string text);
}