public class CallbackData
{
    public CallbackActionType CallbackActionType { get; init; }
    public string ID { get; init; } = null!;
    public string Feed { get; init; } = null!;
    public string? PublicationType { get; init; }
    public string Text { get; init; } = null!;

    public string ToCallback() => (int)CallbackActionType + ID;

    public static (CallbackActionType ActionType, string ID) FromCallback(string callback)
        => ((CallbackActionType)int.Parse(callback[0].ToString()), callback[1..]);
}

public enum CallbackActionType
{
    SendPublicationTypes = 1,
    Subscribe = 2,
    Unsubscribe = 3
}
