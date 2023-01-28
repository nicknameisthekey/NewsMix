using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Models;

namespace NewsMix.Feeds;

public class NoobClubFeed : Feed
{
    #region publication types consts
    public const string overwatchPubType = "overwatch";
    public const string wowPubType = "wow";
    public const string wowClassicPubType = "wow_classic";
    public const string hsPubType = "hearthstone";
    public const string wc3PubType = "warcraft 3";
    public const string blizzPubType = "blizzard";
    public const string diabloPubType = "diablo";

    public string[] AvaliablePublicationTypes => new[] { hsPubType, overwatchPubType, wowClassicPubType, wowPubType, wc3PubType, blizzPubType, diabloPubType };
    #endregion

    public string FeedName => "noobclub";
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

        _logger.LogItemsFetched(result.Count, FeedName);
        return result;
    }

    private FeedItem ParseNode(HtmlNode node)
    {
        var titleNode = node.SelectSingleNode($"span[1]/h1/a");

        var aritcleUrl = titleNode.Attributes
            .SingleOrDefault(a => a.Name == "href")?.Value;
        var title = titleNode.InnerText;
        var gameImageNodeClasses = node.SelectSingleNode("span[1]/span[2]")?
            .GetClasses() ?? Array.Empty<string>();

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
            "game-icon owch" => overwatchPubType,
            "game-icon wow" => wowPubType,
            "game-icon wowc" => wowClassicPubType,
            "game-icon hearthstone" => hsPubType,
            "game-icon wc3" => wc3PubType,
            "game-icon blizzard" => blizzPubType,
            "game-icon diablo" => diabloPubType,
            _ => "unknown"
        };
    }
}