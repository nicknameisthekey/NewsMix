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

    public SourcesService(IEnumerable<Source> sources,
    PublicationRepository publicationRepository,
    UserService userService,
    IEnumerable<UserInterface> userInterfaces,
    ILogger<SourcesService>? logger = null)
    {
        _sources = sources;
        _publicationRepository = publicationRepository;
        _userService = userService;
        _userInterfaces = userInterfaces;
        _logger = logger;
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
                            .UsersToNotify(new(source.SourceName, publication.Topic));
                        foreach (var user in usersToNotify)
                        {
                            var userInterface = _userInterfaces.FirstOrDefault(i => i.UIType == user.UIType);
                            if (userInterface != null)
                            {
                                await userInterface.NotifyUser(user: user.UserId, publication.Url);
                                _logger?.LogWarning("Notified user {userId}, publication {publication}", user, publication);
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