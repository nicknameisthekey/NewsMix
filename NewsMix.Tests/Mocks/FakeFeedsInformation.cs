using NewsMix.Abstractions;

public partial class TelegramTests
{
    class FakeSourcesInformation : SourcesInformation
    {
        public Dictionary<string, string[]> TopicsBySources { get; init; }
        public string[] Sources { get; init; }

        public FakeSourcesInformation()
        {
            Sources = new[] { "source1", "source2" };
            TopicsBySources = new Dictionary<string, string[]>
            {
                ["source1"] = new[] { "topic1", "topic2" }
            };
        }
    }
}