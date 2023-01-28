using NewsMix.Sources;

namespace NewsMix.Tests;

public class SourcesTests
{
    [Fact]
    public async Task NoobClub()
    {
        var nk = new NoobClub(new FakeDataDownloader(new Dictionary<string, string>
        {
            ["https://www.noob-club.ru/index.php?frontpage;p=0"] = "nk_main.html"

        }));

        var items = await nk.GetPublications();
        Assert.Equal(15, items.Count);
        var wowItems = items.Where(i => i.Topic == Sources.NoobClub.Topic_wow);
        Assert.Equal(9, wowItems.Count());

        var diabloItems = items.Where(i => i.Topic == Sources.NoobClub.Topic_diablo);
        Assert.Equal(3, diabloItems.Count());

        var wc3Items = items.Where(i => i.Topic == Sources.NoobClub.Topic_w3);
        Assert.Single(wc3Items);
    }

    [Fact]
    public async Task IcyVeins_parsing()
    {
        var iv = new IcyVeins(new FakeDataDownloader(new Dictionary<string, string>
        {
            ["https://www.icy-veins.com"] = "icy-veins-main.html"

        }));

        var items = await iv.GetPublications();
        Assert.Equal(30, items.Count);

        var wowItems = items.Where(i => i.Topic == IcyVeins.Topic_wow);
        Assert.Equal(24, wowItems.Count());

        var diabloItems = items.Where(i => i.Topic == IcyVeins.Topic_diablo);
        Assert.Equal(3, diabloItems.Count());

        var wc3Items = items.Where(i => i.Topic == IcyVeins.Topic_warcraft);
        Assert.Single(wc3Items);

        var wowClassicItems = items.Where(i => i.Topic == IcyVeins.Topic_wow_classic);
        Assert.Single(wowClassicItems);

        var lostArkItems = items.Where(i => i.Topic == IcyVeins.Topic_lost_arc);
        Assert.Single(lostArkItems);
    }
}