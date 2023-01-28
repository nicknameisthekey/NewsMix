namespace NewsMix.Models;

#pragma warning disable CS0659 //no GetHashcode()
public class Subscription
{
    public string Source { get; init; } = null!;
    public string Topic { get; init; } = null!;

    public Subscription(string source, string topic)
    {
        Source = source;
        Topic = topic;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Subscription sub)
        {
            return sub.Source == Source && sub.Topic == Topic;
        }

        return base.Equals(obj);
    }

    public override string ToString() => $"source: {Source}, topic: {Topic}";
}