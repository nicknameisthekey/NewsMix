namespace NewsMix.Abstractions;
public interface PublicationRepository
{
    Task<bool> IsPublicationNew(string publicationUniqeID);
    Task AddToPublicationNotifiedList(string publicationUniqeID);
}