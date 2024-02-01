using System.ServiceModel.Syndication;
using NewsMix.Models;

namespace NewsMix.Abstractions;

public interface DataDownloader 
{
    Task<IEnumerable<SyndicationItem>> GetFromRSS(string url);
    Task<Page> GetPage(string uri, DownloadMethod downloadMethod);
    Task<byte[]?> DownloadFile(string url);
}
