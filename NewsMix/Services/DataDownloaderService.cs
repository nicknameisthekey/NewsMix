using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Models;
// using OpenQA.Selenium.Chrome;
// using OpenQA.Selenium.Remote;

namespace NewsMix.Services;

public class DataDownloaderService : DataDownloader
{
    /*
     private RemoteWebDriver __driver;
     private RemoteWebDriver driver
     {
         get
         {
             __driver ??= ConnectToSelenium();
             return __driver;
         }
     }

     private Page GetWithSelenium(string url)
     {
         driver.Navigate().GoToUrl(url);
         return new Page
         {
             RawHTML = driver.PageSource,
             FailedToLoad = false
         };
     }

     private RemoteWebDriver ConnectToSelenium()
     {
         _logger?.LogInformation("Connecting to selenium");
         var result = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"),
         new ChromeOptions());
         _logger?.LogInformation("Selenium OK");
         return result;
     }
     */
    private readonly ILogger<DataDownloaderService>? _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public DataDownloaderService(IHttpClientFactory httpClientFactory, ILogger<DataDownloaderService>? logger = null)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Page> GetPage(string uri, DownloadMethod downloadMethod)
    {
        try
        {
            return downloadMethod switch
            {
                //DownloadMethod.Selenium_chromium => GetWithSelenium(uri),
                DownloadMethod.HttpClient => await GetWithHttpClient(uri),
                _ => throw new NotImplementedException()
            };
        }
        catch (Exception e)
        {
            _logger?.LogError(e, "could now download uri: {uri}");
            return Page.FailedToLoadPage;
        }
    }

    public async Task<Page> GetWithHttpClient(string url)
    {
        using var client = _httpClientFactory.CreateClient();
        var result = await client.GetAsync(url);
        var content = await result.Content.ReadAsStringAsync();
        if (result.IsSuccessStatusCode == false)
        {
            _logger?.LogWarning("failed to download page on {url}: {statusCode}, {response}", url, result.StatusCode, content);
            return Page.FailedToLoadPage;
        }
        return new Page(content);
    }

    public Task<IEnumerable<SyndicationItem>> GetFromRSS(string url)
    {
        XmlReader reader = XmlReader.Create(url);
        SyndicationFeed feed = SyndicationFeed.Load(reader);
        return Task.FromResult(feed.Items);
    }

    public async Task<byte[]?> DownloadFile(string url)
    {
        using var client = _httpClientFactory.CreateClient();
        var result = await client.GetAsync(url);
        if (result.IsSuccessStatusCode)
        {
            var file = await result.Content.ReadAsByteArrayAsync();
            if (file.Length > 100)
                return file;
        }

        return null;
    }
}