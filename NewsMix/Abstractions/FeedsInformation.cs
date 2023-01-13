namespace NewsMix.Abstractions;

public interface FeedsInformation
{
    Dictionary<string, string[]> PublicationTypesByFeed { get; init; }
    string[] Feeds { get; init; }
}