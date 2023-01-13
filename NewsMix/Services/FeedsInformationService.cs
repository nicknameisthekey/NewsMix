using NewsMix.Abstractions;

public class FeedsInformationService : FeedsInformation
{
    public Dictionary<string, string[]> PublicationTypesByFeed { get; init; }

    public string[] Feeds { get; init; }

    public FeedsInformationService(IEnumerable<Feed> feeds)
    {
        PublicationTypesByFeed = feeds
        .ToDictionary(g => g.FeedName, f => f.AvaliablePublicationTypes);

        Feeds = feeds.Select(f=>f.FeedName).ToArray();
    }
}