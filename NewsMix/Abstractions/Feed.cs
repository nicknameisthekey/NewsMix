using Microsoft.Extensions.Logging;

namespace NewsMix.Abstractions
{
    public interface Feed
    {
        string FeedName { get; }
        string[] AvaliablePublicationTypes { get; }
        Task<IReadOnlyCollection<FeedItem>> GetItems();
    }

    public class FeedItem
    {
        public string Text { get; init; } = null!;
        public string Url { get; init; } = null!;
        public string PublicationType { get; init; } = null!;
    }
}