using System.Globalization;
using System.Xml.Linq;
using EnableDevMode.Data;

namespace EnableDevMode.Services;

/// <summary>Builds /roundup/feed.xml, an RSS feed of every issue with its full text.</summary>
public static class RoundupFeed
{
    static readonly XNamespace Atom = "http://www.w3.org/2005/Atom";
    static readonly XNamespace Content = "http://purl.org/rss/1.0/modules/content/";

    public static IResult Build(HttpContext http, IEnumerable<RoundupIssue> issues)
    {
        var root = $"{http.Request.Scheme}://{http.Request.Host}";
        var list = issues.ToList();

        var channel = new XElement("channel",
            new XElement("title", "The Blazor Roundup"),
            new XElement("link", root + "/roundup"),
            new XElement("description", "What shipped in Blazor and .NET, and what to do about it. Every other Thursday from Enable Dev Mode."),
            new XElement("language", "en"),
            new XElement(Atom + "link",
                new XAttribute("href", root + "/roundup/feed.xml"),
                new XAttribute("rel", "self"),
                new XAttribute("type", "application/rss+xml")));

        if (list.Count > 0)
            channel.Add(new XElement("lastBuildDate", Rfc822(list[0].Sent)));

        foreach (var issue in list)
        {
            var url = root + issue.Url;
            channel.Add(new XElement("item",
                new XElement("title", $"#{issue.Number}: {issue.Subject}"),
                new XElement("link", url),
                new XElement("guid", new XAttribute("isPermaLink", "true"), url),
                new XElement("pubDate", Rfc822(issue.Sent)),
                new XElement("description", issue.Preview),
                new XElement(Content + "encoded", new XCData(issue.Html))));
        }

        var rss = new XDocument(new XDeclaration("1.0", "utf-8", null),
            new XElement("rss",
                new XAttribute("version", "2.0"),
                new XAttribute(XNamespace.Xmlns + "atom", Atom),
                new XAttribute(XNamespace.Xmlns + "content", Content),
                channel));

        return Results.Text(rss.Declaration + "\n" + rss.ToString(), "application/rss+xml; charset=utf-8");
    }

    // Only the day is known; noon UTC keeps it on the same date from the Americas to Asia.
    static string Rfc822(DateOnly day) =>
        day.ToDateTime(new TimeOnly(12, 0), DateTimeKind.Utc).ToString("r", CultureInfo.InvariantCulture);
}
