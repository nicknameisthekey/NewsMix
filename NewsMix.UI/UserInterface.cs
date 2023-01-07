namespace NewsMix.UI;
public interface UserInterface
{
    string UIType { get; }
    Task Start();
    Task NotifyUser(string user, string message);
}