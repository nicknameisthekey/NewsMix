namespace NewsMix.Abstractions;

public interface IStatsService
{
    Task<int> UsersCount();
    Task<int> NotificationsCount();
}