using System.Text;
using NewsMix.UI.Telegram.Models;
using Newtonsoft.Json;

namespace NewsMix.UI.Telegram;
public class TelegramApi
{
    private const string apiBaseUrl = "https://api.telegram.org/bot";
    private readonly string getUpdatesUrl;
    private readonly string sendMessageUrl;
    private long lastUpdateId = 0;
    private readonly IHttpClientFactory _httpClientFactory;
    public TelegramApi(string token, IHttpClientFactory httpClientFactory)
    {
        getUpdatesUrl = apiBaseUrl + token + "/getUpdates";
        sendMessageUrl = apiBaseUrl + token + "/sendMessage";
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<Update>> GetUpdates()
    {
        var request = new GetUpdatesRequest { Offset = lastUpdateId + 1 };
        var response = await PostAsync<GetUpdatesResponse>(request, getUpdatesUrl);
        response.Updates = response.Updates.OrderBy(u => u.Id).ToList();
        lastUpdateId = response.Updates.LastOrDefault()?.Id ?? lastUpdateId;
        return response.Updates;
    }

    internal async Task SendMessage(SendMessageRequest message)
    {
        await PostAsync<SendMessageResponse>(message, sendMessageUrl);
    }
    
    internal async Task<T> PostAsync<T>(object requestContent, string url)
    {
        JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        string requestSerialized = JsonConvert.SerializeObject(requestContent, jsonSettings);
        Console.WriteLine(requestSerialized);
        var data = new StringContent(requestSerialized, Encoding.UTF8, "application/json");
        using var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync(url, data);
        var responseString = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseString);
        var responseDeserialized = JsonConvert.DeserializeObject<T>(responseString);
        return responseDeserialized;
    }
}