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
        return await _userRepository.UsersCount();
    }

    public async Task<int> NotificationsCount()
    {
        return await _publicationRepository.NotificationCount();
    }
}