using System.Diagnostics;
using HtmlAgilityPack;
using NewsMix.Abstractions;

namespace NewsMix.Feeds;

public class NoobClubFeed : Feed
{
    const string overwatchFeedItemType = "overwatch";
    const string wowFeedItemType = "wow";
    public const string wowClassicFeedItemType = "wow_classic";
    public const string hearthstoneFeedItemType = "hearthstone";

    public string FeedName => "noob-club";

    public string[] AvaliablePublicationTypes => new[] { hearthstoneFeedItemType, overwatchFeedItemType, wowClassicFeedItemType, wowFeedItemType };

    const string siteUrl = "https://www.noob-club.ru";
    static readonly Dictionary<int, string> pagesUrls = new();
    static NoobClubFeed()
    {
        for (int i = 1; i <= 15; i++)
            pagesUrls.Add(i, $"{siteUrl}/index.php?frontpage;p={(i - 1) * 15}");
    }

    public async Task<IReadOnlyCollection<FeedItem>> GetItems()
    {
        var result = new List<FeedItem>();
        using var httpClient = new HttpClient();
        foreach (var (page, url) in pagesUrls)
        {
            Debug.WriteLine($"loading page {page}");
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode == false)
            {
                Debug.WriteLine($"got bad http code {response.StatusCode}");
                return result;
            }

            var html = await response.Content.ReadAsStringAsync();
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var nodes = htmlDocument.DocumentNode
                .SelectNodes($"//*[@class=\"entry first\"]");
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