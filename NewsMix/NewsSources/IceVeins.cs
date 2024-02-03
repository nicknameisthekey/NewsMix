using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Models;

namespace NewsMix.NewsSources;
public class IcyVeins : NewsSource
{
    #region topics
    public const string Topic_wow = "wow";
    public const string Topic_wow_classic = "wow-classic";
    public const string Topic_diablo = "diablo";
    public const string Topic_warcraft = "warcraft 3";
    public const string Topic_lost_arc = "lost ark";
    #endregion

    public string Name => "icyveins";
    const string siteUrl = "https://www.icy-veins.com";
    private readonly DataDownloader _dataDownloader;
    private readonly ILogger<IcyVeins>? _logger;

    public IcyVeins(DataDownloader dataDownloader, ILogger<IcyVeins>? logger = null)
    {
        _dataDownloader = dataDownloader;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Publication>> GetPublications()
    {
        var result = new List<Publication>();

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
            if (nodeData != null)
                result.Add(nodeData);
        }

        return result;
    }

    private Publication? ParseNode(HtmlNode node)
    {
        var titleNode = node.SelectSingleNode($"span[2]/span/span[1]/a");

        var url = titleNode.Attributes
            .SingleOrDefault(a => a.Name == "href")?.Value;
        if (string.IsNullOrEmpty(url))
            return null;

        var title = titleNode.InnerText;
        var gameName = node.SelectSingleNode("span[2]/span/span[3]/span[1]")?.InnerText;

        var topic = gameName switch
        {
            "World of Warcraft" => Topic_wow,
            "Diablo Immortal" => Topic_diablo,
            "Diablo" => Topic_diablo,
            "WotLK Classic" => Topic_wow_classic,
            "Warcraft Reforged" => Topic_warcraft,
            "Lost Ark" => Topic_lost_arc,
            _ => "unknown"
        };

        return new Publication
        {
            Text = title,
            Url = url,
            TopicInternalName = topic,
            Source = Name
        };
    }
}