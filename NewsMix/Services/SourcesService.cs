using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;

namespace NewsMix.Services;
public class SourcesService : BackgroundService
{
    private readonly IEnumerable<Source> _sources;
    private readonly PublicationRepository _publicationRepository;
    private readonly UserService _userService;
    private readonly IEnumerable<UserInterface> _userInterfaces;
    private readonly ILogger<SourcesService>? _logger;

    public SourcesService(IServiceProvider services)
    {
        var scope = services.CreateScope();
        
        _sources = scope.ServiceProvider.GetRequiredService<IEnumerable<Source>>();
        _publicationRepository = scope.ServiceProvider.GetRequiredService<PublicationRepository>();
        _userService = scope.ServiceProvider.GetRequiredService<UserService>();
        _userInterfaces = scope.ServiceProvider.GetRequiredService<IEnumerable<UserInterface>>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<SourcesService>>();
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            foreach (var source in _sources)
            {
                var publications = await source.GetPublications();

                _logger.LogPublicationsFetched(publications.Count, source.SourceName);

                foreach (var publication in publications)
                {
                    if (await _publicationRepository.IsPublicationNew(publication.Url))
                    {
                        var usersToNotify = await _userService
                            .UsersToNotify(new Storage.Entites.Subscription
                            {
                                Source = source.SourceName,
                                Topic = publication.Topic
                            });
                        foreach (var user in usersToNotify)
                        {
                            var userInterface = _userInterfaces.FirstOrDefault(i => i.UIType == user.UIType);
                            if (userInterface != null)
                            {
                                await userInterface.NotifyUser(user: user.UserId, publication.Url);
                                _logger?.LogWarning("Notified user {user}, publication {publication}", user, publication);
                            }
                        }
                    }
                    await _publicationRepository.SetPublicationNotified(publication.Url);
                }
            }
            await Task.Delay(TimeSpan.FromMinutes(10));
        }
    }
}