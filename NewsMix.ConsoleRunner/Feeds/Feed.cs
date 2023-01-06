public interface Feed
{
    Task<IReadOnlyCollection<FeedItem>> GetItems();
}

public class FeedItem
{
    public string Text { get; init; } = null!;
    public string Url { get; init; } = null!;
    public string Type { get; init; } = null!;
}