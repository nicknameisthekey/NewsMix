using NewsMix.Feeds;

namespace NewsMix.Tests;

public class FeedsTests
{
    [Fact]
    public async Task NoobClub()
    {
        var nk = new NoobClubFeed(new FakeDataDownloader(new Dictionary<string, string>
        {
            ["https://www.noob-club.ru/index.php?frontpage;p=0"] = "nk_main.html"

        }));

        var items = await nk.GetItems();
        Assert.Equal(15, items.Count);
        var wowItems = items.Where(i=>i.PublicationType == NoobClubFeed.wowFeedItemType);
        Assert.Equal(10, wowItems.Count());
    }
}