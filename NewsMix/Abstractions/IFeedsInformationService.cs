public interface IFeedsInformationService
{
    Dictionary<string, string[]> PublicationTypesByFeed { get; init; }
    string[] Feeds { get; init; }
}