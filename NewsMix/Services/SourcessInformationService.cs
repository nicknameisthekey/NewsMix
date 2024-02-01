using NewsMix.Abstractions;

namespace NewsMix.Services;

public class SourcessInformationService : SourcesInformation
{
    public Dictionary<string, string[]> TopicsBySources { get; init; }

    public string[] Sources { get; init; }

    public SourcessInformationService(IEnumerable<Source> sources)
    {
        TopicsBySources = sources
            .ToDictionary(g => g.SourceName, f => f.Topics);

        Sources = sources.Select(f => f.SourceName).ToArray();
    }
}