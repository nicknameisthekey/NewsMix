using NewsMix.Abstractions;

public class HabrFeed : Feed
{
    const string rssUrl = "https://habr.com/en/rss/all/top25/?fl=en%2Cru";
    private const string rating25PubType = "Rating > 25";

    public string FeedName => "habr";

    public string[] AvaliablePublicationTypes => new[] { rating25PubType };

    private readonly DataDownloader _dataDownloader;

    public HabrFeed(DataDownloader dataDownloader)
    {
        _dataDownloader = dataDownloader;
    }

    public async Task<IReadOnlyCollection<FeedItem>> GetItems()
    {
        var items = await _dataDownloader.GetFromRSS(rssUrl);
        return items.Select(i => new FeedItem
        {
            Url = i.Id,
            Text = "",
            PublicationType = rating25PubType
        }).ToList();
    }
}