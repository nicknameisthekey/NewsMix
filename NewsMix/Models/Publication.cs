namespace NewsMix.Models;

public record class Publication
{
    public string Text { get; init; } = null!;
    public string Url { get; init; } = null!;
    public string Topic { get; init; } = null!;
}