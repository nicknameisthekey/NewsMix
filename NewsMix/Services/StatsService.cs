using NewsMix.Abstractions;

namespace NewsMix.Services;

public class StatsService : IStatsService
{
    private readonly UserRepository _userRepository;
    private readonly PublicationRepository _publicationRepository;

    public StatsService(PublicationRepository publicationRepository, UserRepository userRepository)
    {
        _publicationRepository = publicationRepository;
        _userRepository = userRepository;
    }

    public async Task<int> UsersCount()
    {
        var users = await _userRepository.GetUsers();
        return users.Count;
    }

    public async Task<int> NotificationsCount()
    {
        return await _publicationRepository.NotificationCount();
    }
}