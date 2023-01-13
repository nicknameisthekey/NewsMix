using NewsMix.Models;

namespace NewsMix.Abstractions;

public interface DataDownloader
{
    Task<Page> GetPage(string uri, DownloadMethod downloadMethod);
    Task<byte[]?> DownloadFile(string url);
}
