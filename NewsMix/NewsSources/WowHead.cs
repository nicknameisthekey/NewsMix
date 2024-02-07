using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Models;

namespace NewsMix.NewsSources;

public class WowHead(DataDownloader dataDownloader, ILogger<WowHead>? logger = null) : NewsSource
{
    public string Name => "wowhead";
    private const string rssUrl = "https://www.wowhead.com/news/rss/all";

    public const string Topic_wow_classic = "wow_classic";
    public const string Topic_wow_classic_series = "wow_classic_series";
    public const string Topic_wotlk = "wotlk";
    public const string Topic_cata = "cata";
    public const string Topic_diablo_immortal = "diablo-immortal";
    public const string Topic_diablo_4 = "diablo-4";
    public const string Topic_other = "other";
    public const string Topic_diablo = "diablo";
    public const string Topic_wow = "wow";
    public const string Topic_in_dev = "in-development";

    public readonly Dictionary<string, string> rssFeeds = new()
    {
        [Topic_wow_classic] = "https://www.wowhead.com/news/rss/classic-series",
        [Topic_wow_classic_series] = "https://www.wowhead.com/news/rss/classic",
        [Topic_wotlk] = "https://www.wowhead.com/news/rss/wotlk-classic",
        [Topic_cata] = "https://www.wowhead.com/news/rss/cata-classic",
        [Topic_diablo_immortal] = "https://www.wowhead.com/news/rss/diablo-immortal",
        [Topic_diablo_4] = "https://www.wowhead.com/news/rss/diablo-4",
        [Topic_wow] = "https://www.wowhead.com/news/rss/retail",
        [Topic_in_dev] = "https://www.wowhead.com/news/rss/in-dev",
        [Topic_other] = "https://www.wowhead.com/news/rss/other-blizzard-games",
        [Topic_diablo] = "https://www.wowhead.com/news/rss/diablo",
    };

    public async Task<IReadOnlyCollection<Publication>> GetPublications()
    {
        var result = new List<Publication>();
        foreach (var kvp in rssFeeds)
        {
            var news = await dataDownloader.GetFromRSS(kvp.Value);
            result.AddRange(news.Select(s => new Publication()
            {
                Source = Name,
                TopicInternalName = kvp.Key,
                Url = s.Id
            }));
        }

        return result;
    }
}