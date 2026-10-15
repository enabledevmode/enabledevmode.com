namespace EnableDevMode.Data;

/// <summary>
/// One sent issue of The Blazor Roundup, read from <c>Content/roundup/NNN.md</c>.
/// </summary>
/// <param name="Number">The issue number; also the page address, /roundup/{Number}.</param>
/// <param name="Sent">The day it went out by email.</param>
/// <param name="Subject">The headline: the page title and the card title.</param>
/// <param name="Preview">One sentence under the headline; also the page's description for search and link cards.</param>
/// <param name="Html">The body, rendered from Markdown.</param>
/// <param name="Highlights">Up to three one-line highlights for the home page's latest-issue card. Optional.</param>
/// <param name="Tags">A few short topic labels for the archive. Optional.</param>
public sealed record RoundupIssue(
    int Number,
    DateOnly Sent,
    string Subject,
    string Preview,
    string Html,
    IReadOnlyList<string> Highlights,
    IReadOnlyList<string> Tags)
{
    /// <summary>The issue's address on the site.</summary>
    public string Url => $"/roundup/{Number}";
}
