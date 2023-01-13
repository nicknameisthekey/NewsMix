using System.Text;
using Microsoft.Extensions.Configuration;
using NewsMix.UI.Telegram.Models;
using Newtonsoft.Json;

namespace NewsMix.UI.Telegram;
public class TelegramApi : ITelegramApi
{
    private const string apiBaseUrl = "https://api.telegram.org/bot";
    private readonly string getUpdatesUrl;
    private readonly string sendMessageUrl;
    private readonly string editMessageTextUrl;
    private long lastUpdateId = 0;
    private readonly IHttpClientFactory _httpClientFactory;
    public TelegramApi(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        var token = configuration["TelegramBotToken"] ?? throw new ArgumentNullException();
        getUpdatesUrl = apiBaseUrl + token + "/getUpdates";
        sendMessageUrl = apiBaseUrl + token + "/sendMessage";
        editMessageTextUrl = apiBaseUrl + token + "/editMessageText";
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

    public async Task<SendMessageResponse> SendMessage(SendMessageRequest message)
    {
        return await PostAsync<SendMessageResponse>(message, sendMessageUrl);
    }

    public async Task<SendMessageResponse> EditMessage(EditMessageText message)
    {
        return await PostAsync<SendMessageResponse>(message, editMessageTextUrl);
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