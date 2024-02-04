namespace NewsMix.Models;

public record class Publication
{
    public string Source { get; set; } = null!;
    public string Text { get; init; } = null!;
    public string Url { get; init; } = null!;
    public string TopicInternalName { get; init; } = null!;
}