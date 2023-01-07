using Microsoft.Extensions.Hosting;
using NewsMix.DAL.Repositories.Abstraction;
using NewsMix.Feeds;
using NewsMix.UI;

namespace NewsMix.Core.Services;
public class FeedService : IHostedService
{
    private readonly IEnumerable<Feed> _feeds;
    private readonly PublicationRepository _publicationRepository;
    private readonly UserService _userService;
    private readonly IEnumerable<UserInterface> _userInterfaces;
    public FeedService(IEnumerable<Feed> feeds,
    PublicationRepository publicationRepository,
    UserService userService,
    IEnumerable<UserInterface> userInterfaces)
    {
        _feeds = feeds;
        _publicationRepository = publicationRepository;
        _userService = userService;
        _userInterfaces = userInterfaces;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var ui in _userInterfaces)
        {
            ui.Start();
        }

        while (cancellationToken.IsCancellationRequested == false)
        {
            foreach (var feed in _feeds)
            {
                var items = await feed.GetItems();
                foreach (var item in items)
                {
                    if (await _publicationRepository.IsPublicationNew(item.Url))
                    {
                        var usersToNotify = await _userService
                            .GetUsersToNotifyBy(feedName: feed.FeedName,
                                            publicationType: item.PublicationType);
                        foreach (var user in usersToNotify)
                        {
                            var userInterface = _userInterfaces.FirstOrDefault(i => i.UIType == user.UIType);
                            if (userInterface != null)
                                await userInterface.NotifyUser(user: user.UserId, item.Url);
                        }
                    }
                    await _publicationRepository.AddToPublicationNotifiedList(item.Url);
                }
            }
            await Task.Delay(TimeSpan.FromMinutes(10));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}