using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NewsMix.Services;
using NewsMix.Storage;

public static partial class TestHelpers
{
    public static UserService NewUserService
        => new UserService(CreateDb().repo);

    public static (SqliteRepository repo, SqliteContext ctx) CreateDb()
    {
        var optionsBuilder = new DbContextOptionsBuilder<SqliteContext>();
        var file = Path.Combine(TestHelpers.fileStoragePath + "test.db");
        optionsBuilder.UseSqlite($"Data Source={file}");
        var ctx = new SqliteContext(optionsBuilder.Options);
        ctx.Database.EnsureDeleted();
        ctx.Database.EnsureCreated();
        var repo = new SqliteRepository(ctx);
        return (repo, ctx);
    }

    public static readonly string fileStoragePath = GetTestFilesDirectory();

    public static IConfiguration MockIConfiguration => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {{"FileDbPath", fileStoragePath},}!).Build();

    public static string EmptyTestFilesDirectory()
    {
        var directory = GetTestFilesDirectory();

        EmptyDirectory(directory);

        return directory;
    }

    public static string GetTestFilesDirectory()
    {
        return Path.Combine(Assembly.GetExecutingAssembly()
                             .Location.Replace("NewsMix.Tests.dll", ""), "file_repo_tests");
    }

    private static void EmptyDirectory(string directory)
    {
        if (Directory.Exists(directory) == false)
            Directory.CreateDirectory(directory);

        foreach (var file in Directory.EnumerateFiles(directory))
        {
            File.Delete(file);
        }
    }
}