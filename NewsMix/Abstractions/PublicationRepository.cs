namespace NewsMix.Abstractions;

public interface PublicationRepository
{
    Task<bool> IsPublicationNew(string publicationUniqeID);
    Task SetPublicationNotified(string publicationUniqeID);
    Task<int> NotificationCount();
}