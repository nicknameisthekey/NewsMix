using NewsMix.Abstractions;

namespace NewsMix.Services;

public class SourcessInformationService : SourcesInformation
{
    public Dictionary<string, string[]> TopicsBySources { get; init; }

    public string[] Sources { get; init; }

    public SourcessInformationService(IEnumerable<NewsSource> sources)
    {
        TopicsBySources = sources
            .ToDictionary(g => g.Name, f => f.Topics);

        Sources = sources.Select(f => f.Name).ToArray();
    }
}