using NewsMix.Models;

namespace NewsMix.Abstractions;

public interface Source
{
    string SourceName { get; }
    string[] Topics { get; }
    Task<IReadOnlyCollection<Publication>> GetPublications();
}