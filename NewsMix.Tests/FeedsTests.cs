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
        var wowItems = items.Where(i=>i.PublicationType == NoobClubFeed.wowPubType);
        Assert.Equal(9, wowItems.Count());

        var diabloItems = items.Where(i=>i.PublicationType == NoobClubFeed.diabloPubType);
        Assert.Equal(3, diabloItems.Count());

        var wc3Items = items.Where(i=>i.PublicationType == NoobClubFeed.wc3PubType);
        Assert.Single(wc3Items);
    }
}