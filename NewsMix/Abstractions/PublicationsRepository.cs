using NewsMix.Models;

namespace NewsMix.Abstractions;

public interface PublicationsRepository
{
    Task<bool> IsPublicationNew(string publicationUrl);
    Task AddPublication(string publicationUrl);
    Task CreateNotificationTasks(Publication publication);
    Task<int> NotificationCount();
}