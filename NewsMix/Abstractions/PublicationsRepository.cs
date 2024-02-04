using NewsMix.Models;

namespace NewsMix.Abstractions;

public interface PublicationsRepository
{
    Task<bool> IsPublicationNew(string publicationUrl);
    Task AddPublication(Publication publication);
    Task CreateNotificationTasks(Publication publication);
    Task<int> NotificationCount();
}