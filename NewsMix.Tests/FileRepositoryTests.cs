using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace NewsMix.Tests;

public class UnitTest1
{
    static readonly string assemblyPath = Assembly.GetExecutingAssembly()
                                          .Location.Replace("NewsMix.Tests.dll", "");
    [Fact]
    public async Task AddToPublicationNotifiedList_doesnt_add_duplicates()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {{"FileDbPath", assemblyPath},}!).Build();

        var repo = new FileRepository(configuration);
        File.Create(repo._publicationNotifiedListTxtFile).Close();

        const string someValue = "this is value";
        await repo.AddToPublicationNotifiedList(someValue);
        await repo.AddToPublicationNotifiedList(someValue);

        var fileLines = File.ReadAllLines(repo._publicationNotifiedListTxtFile);
        Assert.Single(fileLines);

        File.Delete(repo._publicationNotifiedListTxtFile);
    }
}