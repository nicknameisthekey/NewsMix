public class CallbackData
{
    public CallbackActionType CallbackActionType { get; init; }
    public string ID { get; init; } = null!;
    public string Source { get; init; } = null!;
    public string Topic { get; init; } = null!;
    public string Text { get; init; } = null!;
}

public enum CallbackActionType
{
    SendTopics = 1,
    Subscribe = 2,
    Unsubscribe = 3
}
