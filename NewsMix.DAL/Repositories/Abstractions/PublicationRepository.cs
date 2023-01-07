namespace NewsMix.DAL.Repositories.Abstraction;
public interface PublicationRepository
{
    Task<bool> IsPublicationNew(string publicationUniqeID);
    Task AddToPublicationNotifiedList(string publicationUniqeID);
}