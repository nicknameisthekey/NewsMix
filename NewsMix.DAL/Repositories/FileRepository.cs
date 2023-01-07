using Microsoft.Extensions.Configuration;
using NewsMix.DAL.Entities;
using NewsMix.DAL.Repositories.Abstraction;
using Newtonsoft.Json;

public class FileRepository : UserRepository, PublicationRepository
{
    private readonly string _baseDbPath;
    private readonly string _usersJsonFile;
    private readonly string _publicationNotifiedListTxtFile;
    public FileRepository(IConfiguration configuration)
    {
        _baseDbPath = configuration["FileDbPath"] ?? throw new ArgumentNullException();
        _usersJsonFile = Path.Combine(_baseDbPath, "users.json");
        _publicationNotifiedListTxtFile = Path.Combine(_baseDbPath, "notified_publications.txt");
    }

    public async Task AddToPublicationNotifiedList(string publicationUniqeID)
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
        return Task.FromResult(JsonConvert.DeserializeObject<List<User>>(usersJson)!);
    }

    public Task UpsertUser(User u)
    {
        var users = GetUsers().Result;
        users.Remove(users.FirstOrDefault(us => us.UserId == u.UserId)!);
        users.Add(u);
        File.WriteAllText(_usersJsonFile, JsonConvert.SerializeObject(users));
        return Task.CompletedTask;
    }
}