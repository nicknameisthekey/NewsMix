using NewsMix.Abstractions;
using NewsMix.Models;

public class FakeDataDownloader : DataDownloader
{
    private readonly Dictionary<string, string> _filesByUrl;
    public FakeDataDownloader(Dictionary<string, string> filesByUrl)
    {
        _filesByUrl = filesByUrl;
    }

    public Task<byte[]?> DownloadFile(string url)
    {
        throw new NotImplementedException();
    }

    public Task<Page> GetPage(string uri, DownloadMethod downloadMethod)
    {
        if (_filesByUrl.ContainsKey(uri) == false)
            return Task.FromResult(Page.FailedToLoadPage);

        string filePath = Path.Combine("FeedsPages", _filesByUrl[uri]);
        string rawHTML = File.ReadAllText(filePath);
        return Task.FromResult(new Page(rawHTML));
    }
}