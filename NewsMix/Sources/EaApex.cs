using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Models;

public class EaApex : Source
{
    public string SourceName => "ea";

    public string[] Topics => new string[] { "Apex" };

    private const string url = "https://www.ea.com/ru-ru/games/apex-legends/news#news";

    private readonly DataDownloader _dataDownloader;
    private readonly ILogger<EaApex>? _logger;

    public EaApex(DataDownloader dataDownloader, ILogger<EaApex>? logger = null)
    {
        _dataDownloader = dataDownloader;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Publication>> GetPublications()
    {
        var page = await _dataDownloader.GetPage(url, DownloadMethod.HttpClient);
        if (page.FailedToLoad)
        {
            _logger?.LogError("failed to load {url}", url);
            return new List<Publication>();
        }

        var allNodes = page.HTMLRoot.SelectNodes("//ea-cta");
        return allNodes.Select(n => GetUrl(n))
            .Where(u => string.IsNullOrEmpty(u) == false)
            .Distinct()
            .Select(u => new Publication
            {
                Url = u!,
                Topic = "Apex"
            }).ToList();
    }

    private string? GetUrl(HtmlNode node)
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