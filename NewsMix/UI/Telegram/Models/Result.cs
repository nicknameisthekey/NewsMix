using Newtonsoft.Json;

public class Result
{
    [JsonProperty("message_id")]
    public long MessageId { get; set; }

            /*
{"ok":true,"result":{"message_id":422,"from":{"id":5688258655,"is_bot":true,"first_name":"NewsMixBot","username":"news_mix_bot"},"chat":{"id":303656773,"first_name":"Vladislav","last_name":"Barsukov","username":"Nicknameisthekey","type":"private"},"date":1673552032,"text":"\u0412\u044b\u0431\u0435\u0440\u0438 \u0438\u0441\u0442\u043e\u0447\u043d\u0438\u043a \u0434\u0430\u043d\u043d\u044b\u0445","reply_markup":{"inline_keyboard":[[{"text":"noob-club","callback_data":"1889fdef9-2f68-47bf-bfd6-ed567e54967e"}]]}}}
        */
}