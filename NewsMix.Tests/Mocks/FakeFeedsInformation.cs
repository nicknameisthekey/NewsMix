using NewsMix.Abstractions;

public partial class TelegramTests
{
    class FakeFeedsInformation : FeedsInformation
    {
        public Dictionary<string, string[]> PublicationTypesByFeed { get; init; }
        public string[] Feeds { get; init; }

        public FakeFeedsInformation()
        {
            Feeds = new[] { "feed1", "feed2" };
            PublicationTypesByFeed = new Dictionary<string, string[]>
            {
                ["feed1"] = new[] { "pub1", "pub2" }
            };
        }
    }
}