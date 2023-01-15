namespace NewsMix.Abstractions;
public interface UserInterface
{
    string UIType { get; }
    Task NotifyUser(string user, string message);
}