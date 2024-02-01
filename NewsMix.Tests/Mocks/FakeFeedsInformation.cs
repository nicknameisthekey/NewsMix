using NewsMix.Abstractions;

namespace NewsMix.Tests.Mocks;

class FakeSourcesInformation : SourcesInformation
{
    public Dictionary<string, string[]> TopicsBySources { get; init; } = new()
    {
        ["source1"] = ["topic1", "topic2"]
    };

    public string[] Sources { get; init; } = ["source1", "source2"];
}