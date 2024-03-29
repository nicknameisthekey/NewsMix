using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Helpers;
using NewsMix.Models;

namespace NewsMix.Services;

public class NewsCollector : BackgroundService
{
    private readonly IEnumerable<NewsSource> _sources;
    private readonly PublicationsRepository _publicationsRepository;
    private readonly ILogger<NewsCollector>? _logger;

    public NewsCollector(IServiceProvider services)
    {
        var scope = services.CreateScope();

        _sources = scope.ServiceProvider.GetRequiredService<IEnumerable<NewsSource>>();
        _publicationsRepository = scope.ServiceProvider.GetRequiredService<PublicationsRepository>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<NewsCollector>>();
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

        while (cancellationToken.IsCancellationRequested == false)
        {
            try
            {
                foreach (var source in _sources)
                {
                    var publications = await GetSafe(source);

                    _logger.LogPublicationsFetched(publications.Count, source.Name);

                    foreach (var publication in publications)
                    {
                        if (await _publicationsRepository.IsPublicationNew(publication.Url))
                            await _publicationsRepository.CreateNotificationTasks(publication);

                        await _publicationsRepository.AddPublication(publication);
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "error while fetching news");
                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
            }
        }
    }

    private async Task<IReadOnlyCollection<Publication>> GetSafe(NewsSource source)
    {
        try
        {
            return await source.GetPublications();
        }
        catch (Exception e)
        {
            _logger?.LogError(e, "error while fetching news");
            return Array.Empty<Publication>();
        }
    }
}