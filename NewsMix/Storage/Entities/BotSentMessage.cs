namespace NewsMix.Storage.Entities;

public class BotSentMessage
{
    public int Id { get; set; }
    public string ExternalUserId { get; set; } = null!;
    public long TelegramMessageId { get; set; }
    public MessageType MessageType { get; set; }
    public DateTime SendAtUTC { get; set; }
    public DateTime? LastUpdated { get; set; }
    public DateTime? DeletedAtUTC { get; set; }
}

public enum MessageType
{
    News, 
    Keyboard,
    Information
}