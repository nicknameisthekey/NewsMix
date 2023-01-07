namespace NewsMix.Feeds;
public interface Feed
{
    string FeedName { get; }
    string[] AvaliablePublicationTypes { get; }
    Task<IReadOnlyCollection<FeedItem>> GetItems();
}
