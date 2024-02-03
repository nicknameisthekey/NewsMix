using NewsMix.Models;

namespace NewsMix.Abstractions;

public interface NewsSource
{
    string Name { get; }
    string[] Topics { get; }
    Task<IReadOnlyCollection<Publication>> GetPublications();
}