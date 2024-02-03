using NewsMix.Abstractions;

namespace NewsMix.Services;

public class StatsService(PublicationsRepository publicationsRepository, UserRepository userRepository) : IStatsService
{
    public async Task<int> UsersCount()
    {
        return await userRepository.UsersCount();
    }

    public async Task<int> NotificationsCount()
    {
        return await publicationsRepository.NotificationCount();
    }
}