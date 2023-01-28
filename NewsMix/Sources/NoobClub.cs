using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Models;

namespace NewsMix.Sources;

public class NoobClub : Source
{
    #region topics
    public const string Topic_overwatch = "overwatch";
    public const string Topic_wow = "wow";
    public const string Topic_wow_classic = "wow_classic";
    public const string Topic_hs = "hearthstone";
    public const string Topic_w3 = "warcraft 3";
    public const string Topic_blizzard = "blizzard";
    public const string Topic_diablo = "diablo";

    public string[] Topics => new[] { Topic_hs, Topic_overwatch, Topic_wow_classic, Topic_wow, Topic_w3, Topic_blizzard, Topic_diablo };
    #endregion

    public string SourceName => "noobclub";
    const string siteUrl = "https://www.noob-club.ru";
    private static readonly Dictionary<int, string> pagesUrls = new();
    private readonly DataDownloader _dataDownloader;
    private readonly ILogger<NoobClub>? _logger;

    static NoobClub()
    {
        for (int i = 1; i <= 15; i++)
            pagesUrls.Add(i, $"{siteUrl}/index.php?frontpage;p={(i - 1) * 15}");
    }

    public NoobClub(DataDownloader dataDownloader, ILogger<NoobClub>? logger = null)
    {
        _dataDownloader = dataDownloader;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Publication>> GetPublications()
    {
        var result = new List<Publication>();
        foreach (var (pageNum, url) in pagesUrls)
        {
            _logger?.LogInformation("{SourceName}: loading page {pageNum}", SourceName, pageNum);

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

    private Publication ParseNode(HtmlNode node)
    {
        var titleNode = node.SelectSingleNode($"span[1]/h1/a");

        var aritcleUrl = titleNode.Attributes
            .SingleOrDefault(a => a.Name == "href")?.Value;
        var title = titleNode.InnerText;
        var gameImageNodeClasses = node.SelectSingleNode("span[1]/span[2]")?
            .GetClasses() ?? Array.Empty<string>();

        var topic = GameImageClassToArticleType(string.Join(" ", gameImageNodeClasses));

        return new Publication
        {
            Text = title,
            Url = siteUrl + aritcleUrl,
            Topic = topic
        };
    }

    private string GameImageClassToArticleType(string gameImageClass)
    {
        return gameImageClass switch
        {
            "game-icon owch" => Topic_overwatch,
            "game-icon wow" => Topic_wow,
            "game-icon wowc" => Topic_wow_classic,
            "game-icon hearthstone" => Topic_hs,
            "game-icon wc3" => Topic_w3,
            "game-icon blizzard" => Topic_blizzard,
            "game-icon diablo" => Topic_diablo,
            _ => "unknown"
        };
    }
}