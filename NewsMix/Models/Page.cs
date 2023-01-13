using HtmlAgilityPack;

namespace NewsMix.Models;

public class Page
{
    private HtmlDocument? _htmlDocument;
    public string? RawHTML { get; init; }
    public bool FailedToLoad { get; init; }
    public HtmlNode HTMLRoot => HTMLDocument.DocumentNode;
    public HtmlDocument HTMLDocument
    {
        get
        {
            if (_htmlDocument == null)
            {
                if (FailedToLoad)
                    throw new Exception("Cannot acces html of failed page");
                _htmlDocument = new HtmlDocument();
                _htmlDocument.LoadHtml(RawHTML);
            }
            return _htmlDocument;
        }
    }

    public Page(string? rawHTML)
    {
        RawHTML = rawHTML;
        if (rawHTML == null || rawHTML.Length < 20)
        {
            FailedToLoad = true;
        }
    }

    public static Page FailedToLoadPage => new Page(null);
}