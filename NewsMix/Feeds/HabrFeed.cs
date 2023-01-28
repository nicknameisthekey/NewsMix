using NewsMix.Abstractions;
using Microsoft.Extensions.Logging;

public class HabrFeed : Feed
{
    const string rssUrl = "https://habr.com/en/rss/all/top25/?fl=en%2Cru";
    private const string rating25PubType = "Rating > 25";

    public string FeedName => "habr";

    public string[] AvaliablePublicationTypes => new[] { rating25PubType };

    private readonly DataDownloader _dataDownloader;
    private readonly ILogger<HabrFeed>? _logger;

    public HabrFeed(DataDownloader dataDownloader, ILogger<HabrFeed>? logger)
    {
        _dataDownloader = dataDownloader;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<FeedItem>> GetItems()
    {
        var items = await _dataDownloader.GetFromRSS(rssUrl);
        _logger.LogItemsFetched(items.Count(), FeedName);
        
        return items.Select(i => new FeedItem
        {
            Url = i.Id,
            Text = "",
            PublicationType = rating25PubType
        }).ToList();
    }
}