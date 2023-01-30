using Microsoft.Extensions.Configuration;
using NewsMix.Abstractions;
using NewsMix.Models;
using Newtonsoft.Json;

namespace NewsMix.Storage;
public class FileRepository : UserRepository, PublicationRepository
{
    private readonly string _baseDbPath;
    internal readonly string _usersJsonFile;
    internal readonly string _publicationNotifiedListTxtFile;
    public FileRepository(IConfiguration configuration)
    {
        _baseDbPath = configuration["FileDbPath"] ?? throw new ArgumentNullException();
        _usersJsonFile = Path.Combine(_baseDbPath, "users.json");
        _publicationNotifiedListTxtFile = Path.Combine(_baseDbPath, "notified_publications.txt");

        CreateFileIfNotExist(_publicationNotifiedListTxtFile);
        CreateFileIfNotExist(_usersJsonFile);
    }

    private void CreateFileIfNotExist(string filePath)
    {
        if (File.Exists(filePath) == false)
            File.Create(filePath).Close();
    }

    public async Task SetPublicationNotified(string publicationUniqeID)
    {
        if (await IsPublicationNew(publicationUniqeID))
            File.AppendAllText(_publicationNotifiedListTxtFile, publicationUniqeID + Environment.NewLine);
        await Task.CompletedTask;
    }

    public Task<bool> IsPublicationNew(string publicationUniqeID)
    {
        var publications = File.ReadLines(_publicationNotifiedListTxtFile);
        return Task.FromResult(publications.Any(p => p == publicationUniqeID) == false);
    }

    public Task<List<User>> GetUsers()
    {
        var usersJson = File.ReadAllText(_usersJsonFile);
        if (usersJson.Length == 0)
            return Task.FromResult(new List<User>());
        else
            return Task.FromResult(JsonConvert.DeserializeObject<List<User>>(usersJson)!);
    }

    public async Task UpsertUser(User u)
    {
        var users = await GetUsers();
        users.Remove(users.FirstOrDefault(us => us.UserId == u.UserId)!);
        users.Add(u);
        File.WriteAllText(_usersJsonFile, JsonConvert.SerializeObject(users));
    }

    public async Task<int> NotificationCount()
    {
       var file = await File.ReadAllLinesAsync(_publicationNotifiedListTxtFile);
       return file.Length;
    }
}