using Microsoft.Extensions.Hosting;
using NewsMix.Abstractions;

namespace NewsMix.Services;
public class BotChangesNotifier : BackgroundService
{
    private readonly IEnumerable<UserInterface> _UIs;
    private readonly UserRepository _userRepo;
    private const string fileName = "notifications_to_do.txt";
    public BotChangesNotifier(IEnumerable<UserInterface> uIs, UserRepository userRepo)
    {
        _UIs = uIs;
        _userRepo = userRepo;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            await Task.Delay(10000);

            if (File.Exists(fileName) == false)
                continue;
            var notification = File.ReadAllText(fileName);
            if (notification == null || notification.Length < 3)
                continue;

            var telegramUI = _UIs.FirstOrDefault(u => u.UIType == "telegram");
            if (telegramUI == null)
                continue;

            var users = await _userRepo.GetUsers();
            File.WriteAllText(fileName, "");

            foreach (var user in users)
            {
                await telegramUI.NotifyUser(user.UserId, notification);
            }
        }
    }
}