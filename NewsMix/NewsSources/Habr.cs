using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Models;

namespace NewsMix.NewsSources;
public class Habr : NewsSource
{
    const string rssUrl = "https://habr.com/en/rss/all/top25/?fl=en%2Cru";
    public const string rating25Topic = "r25+";

    public string Name => "habr";

    private readonly DataDownloader _dataDownloader;
    private readonly ILogger<Habr>? _logger;

    public Habr(DataDownloader dataDownloader, ILogger<Habr>? logger)
    {
        _dataDownloader = dataDownloader;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Publication>> GetPublications()
    {
        var items = await _dataDownloader.GetFromRSS(rssUrl);
       
        return items.Select(i => new Publication
        {
            Url = i.Id,
            Text = "",
            TopicInternalName = rating25Topic,
            Source = Name
        }).ToList();
    }
}