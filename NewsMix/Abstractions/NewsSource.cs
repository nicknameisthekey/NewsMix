using NewsMix.Models;

namespace NewsMix.Abstractions;

public interface NewsSource
{
    string Name { get; }
    Task<IReadOnlyCollection<Publication>> GetPublications();
}