using System.Reflection;
using Microsoft.Extensions.Configuration;

public static partial class TestHelpers
{
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

    private static string GetTestFilesDirectory()
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