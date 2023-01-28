namespace NewsMix.Abstractions;

public interface SourcesInformation
{
    Dictionary<string, string[]> TopicsBySources { get; init; }
    string[] Sources { get; init; }
}