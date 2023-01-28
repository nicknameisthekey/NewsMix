using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Models;

public class IcyVeinsFeed : Feed
{
    #region publication types consts
    public const string wowPubType = "wow";
    public const string wowClassicPubType = "wow-classic";
    public const string diabloPubType = "diablo";
    public const string warcraftPubType = "warcraft 3";
    public const string lostArcPubType = "lost ark";
    public string[] AvaliablePublicationTypes => new[] { wowClassicPubType, wowPubType, diabloPubType, warcraftPubType, lostArcPubType };
    #endregion

    public string FeedName => "icyveins";
    const string siteUrl = "https://www.icy-veins.com";
    private readonly DataDownloader _dataDownloader;
    private readonly ILogger<IcyVeinsFeed>? _logger;

    public IcyVeinsFeed(DataDownloader dataDownloader, ILogger<IcyVeinsFeed>? logger = null)
    {
        _dataDownloader = dataDownloader;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<FeedItem>> GetItems()
    {
        var result = new List<FeedItem>();
        _logger?.LogInformation($"{FeedName}: loading main page");

        var page = await _dataDownloader.GetPage(siteUrl, DownloadMethod.HttpClient);
        if (page.FailedToLoad)
        {
            _logger?.LogWarning("failed to load {url}", siteUrl);
            return result;
        }
        for (int i = 1; i <= 30; i++)
        {
            var node = page.HTMLRoot.SelectSingleNode($"//*[@id=\"news_{i}\"]");
            var nodeData = ParseNode(node);
            result.Add(nodeData);
        }

        return result;
    }

    private FeedItem ParseNode(HtmlNode node)
    {
        var titleNode = node.SelectSingleNode($"span[2]/span/span[1]/a");

        var aritcleUrl = titleNode.Attributes
            .SingleOrDefault(a => a.Name == "href")?.Value;

        var title = titleNode.InnerText;
        var gameText = node.SelectSingleNode("span[2]/span/span[3]/span[1]")?.InnerText;

        var gameType = gameText switch
        {
            "World of Warcraft" => wowPubType,
            "Diablo Immortal" => diabloPubType,
            "Diablo" => diabloPubType,
            "WotLK Classic" => wowClassicPubType,
            "Warcraft Reforged" => warcraftPubType,
            "Lost Ark" => lostArcPubType,
            _ => "unknown"
        };

        return new FeedItem
        {
            Text = title,
            Url = aritcleUrl,
            PublicationType = gameType
        };
    }
}