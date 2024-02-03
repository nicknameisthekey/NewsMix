namespace NewsMix.Abstractions;

public interface UserInterface
{
    string UIName { get; }
    Task NotifyUser(string user, string message);
}