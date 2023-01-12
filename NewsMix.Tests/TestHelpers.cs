using System.Reflection;
using Microsoft.Extensions.Configuration;

public static partial class TestHelpers
{
    public static readonly string fileStoragePath = PrepareFileStorage();

    public static IConfiguration MockIConfiguration => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {{"FileDbPath", fileStoragePath},}!).Build();

    public static string PrepareFileStorage()
    {
        var directory = Path.Combine(Assembly.GetExecutingAssembly()
                     .Location.Replace("NewsMix.Tests.dll", ""), "file_repo_tests");

        EmptyDirectory(directory);

        return directory;
    }

    public static void EmptyDirectory(string directory)
    {
        if (Directory.Exists(directory))
            Directory.Delete(directory, true);

        Directory.CreateDirectory(directory);
    }
}