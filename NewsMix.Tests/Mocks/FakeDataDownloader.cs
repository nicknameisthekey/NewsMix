using System.ServiceModel.Syndication;
using NewsMix.Abstractions;
using NewsMix.Models;

namespace NewsMix.Tests.Mocks;

public class FakeDataDownloader(IReadOnlyDictionary<string, string> filesByUrl) : DataDownloader
{
    public Task<Page> GetPage(string uri, DownloadMethod downloadMethod)
    {
        if (filesByUrl.ContainsKey(uri) == false)
            return Task.FromResult(Page.FailedToLoadPage);

        var filePath = Path.Combine("SourcePages", filesByUrl[uri]);
        var rawHTML = File.ReadAllText(filePath);
        return Task.FromResult(new Page(rawHTML));
    }
    
    public Task<byte[]?> DownloadFile(string url) => throw new NotImplementedException();

    public Task<IEnumerable<SyndicationItem>> GetFromRSS(string url) => throw new NotImplementedException();
}