namespace NewsMix.Abstractions;

public interface PublicationsRepository
{
    Task<bool> IsPublicationNew(string publicationUniqueID);
    Task SetPublicationNotified(string publicationUniqueID);
    Task<int> NotificationCount();
}