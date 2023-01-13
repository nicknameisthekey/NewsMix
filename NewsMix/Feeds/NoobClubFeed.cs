using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Models;

namespace NewsMix.Feeds;

public class NoobClubFeed : Feed
{
    #region publication types consts
    public const string overwatchFeedItemType = "overwatch";
    public const string wowFeedItemType = "wow";
    public const string wowClassicFeedItemType = "wow_classic";
    public const string hearthstoneFeedItemType = "hearthstone";

    public string[] AvaliablePublicationTypes => new[] { hearthstoneFeedItemType, overwatchFeedItemType, wowClassicFeedItemType, wowFeedItemType };
    #endregion

    public string FeedName => "noob-club";
    const string siteUrl = "https://www.noob-club.ru";
    private static readonly Dictionary<int, string> pagesUrls = new();
    private readonly DataDownloader _dataDownloader;
    private readonly ILogger<NoobClubFeed>? _logger;

    static NoobClubFeed()
    {
        for (int i = 1; i <= 15; i++)
            pagesUrls.Add(i, $"{siteUrl}/index.php?frontpage;p={(i - 1) * 15}");
    }

    public NoobClubFeed(DataDownloader dataDownloader, ILogger<NoobClubFeed>? logger = null)
    {
        _dataDownloader = dataDownloader;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<FeedItem>> GetItems()
    {
        var result = new List<FeedItem>();
        foreach (var (pageNum, url) in pagesUrls)
        {
            _logger?.LogInformation($"{FeedName}: loading page {pageNum}");

            var page = await _dataDownloader.GetPage(url, DownloadMethod.HttpClient);
            if (page.FailedToLoad)
            {
                _logger?.LogWarning("failed to load {url}", url);
                continue;
            }

            var nodes = page.HTMLRoot.SelectNodes($"//*[@class=\"entry first\"]");
            foreach (var node in nodes)
            {
                var nodeData = ParseNode(node);
                result.Add(nodeData);
            }
        }

        return result;
    }

    private FeedItem ParseNode(HtmlNode node)
    {
        var titleNode = node.SelectSingleNode($"span[1]/h1/a");

        var aritcleUrl = titleNode.Attributes
            .SingleOrDefault(a => a.Name == "href")?.Value;
        var title = titleNode.InnerText;
        var gameImageNodeClasses = node.SelectSingleNode("span[1]/span[2]")?
            .GetClasses();

        var gameType = GameImageClassToArticleType(string.Join(" ", gameImageNodeClasses));

        return new FeedItem
        {
            Text = title,
            Url = siteUrl + aritcleUrl,
            PublicationType = gameType
        };
    }

    private string GameImageClassToArticleType(string gameImageClass)
    {
        return gameImageClass switch
        {
            "game-icon owch" => overwatchFeedItemType,
            "game-icon wow" => wowFeedItemType,
            "game-icon wowc" => wowClassicFeedItemType,
            "game-icon hearthstone" => hearthstoneFeedItemType,
            _ => "unknown"
        };
    }
}