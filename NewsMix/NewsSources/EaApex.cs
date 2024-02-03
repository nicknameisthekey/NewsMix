using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Models;

namespace NewsMix.NewsSources;

public class EaApex(DataDownloader dataDownloader, ILogger<EaApex>? logger = null) : NewsSource
{
    public string Name => "ea";
    public const string ApexTopic = "apex";

    private const string url = "https://www.ea.com/ru-ru/games/apex-legends/news#news";

    public async Task<IReadOnlyCollection<Publication>> GetPublications()
    {
        var page = await dataDownloader.GetPage(url, DownloadMethod.HttpClient);
        if (page.FailedToLoad)
        {
            logger?.LogError("failed to load {url}", url);
            return new List<Publication>();
        }

        var allNodes = page.HTMLRoot.SelectNodes("//ea-cta");
        return allNodes.Select(GetUrl)
            .Where(u => string.IsNullOrEmpty(u) == false)
            .Distinct()
            .Select(u => new Publication
            {
                Url = u!,
                TopicInternalName = "Apex",
                Source = Name
            }).ToList();
    }

    private static string? GetUrl(HtmlNode node)
    {
        if (node.ChildNodes.Count() != 3)
            return null;

        var attribute = node.ChildNodes[1].GetAttributes()
            .FirstOrDefault(a => a.Name == "href");

        if (attribute == null)
            return null;

        var url = attribute?.Value;

        if (url?.Contains("/games/apex-legends/news/") == false)
            return null;

        return "https://www.ea.com/ru-ru" + url;
    }
}