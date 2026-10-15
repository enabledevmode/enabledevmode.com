using System.Globalization;
using System.Text.RegularExpressions;
using Markdig;
using Markdig.Syntax;

namespace EnableDevMode.Data;

/// <summary>
/// Every sent issue of The Blazor Roundup, newest first, read once at startup from
/// <c>Content/roundup/NNN.md</c>. Adding an issue means adding one file: the home page, the
/// archive, the issue page, the sitemap and the feed all read from here.
/// </summary>
/// <remarks>
/// Each file starts with front matter:
/// <code>
/// ---
/// number: 3
/// sent: 2026-10-22
/// subject: "The subject line as sent"
/// preview: "The preview text as sent"
/// highlights:
///   - Up to three one-line highlights for the home page
/// tags: [Security, Radzen 12.1]
/// ---
/// </code>
/// <c>highlights</c> and <c>tags</c> are optional; each is a list, written either as indented
/// <c>- item</c> lines or as <c>[a, b]</c> on one line.
/// The body is Markdown. Raw HTML is turned off, so nothing in a draft can change the page around
/// it, and a leading # heading is dropped because the page shows the subject as its title. A file
/// that can't be read is logged and left out rather than stopping the site.
/// </remarks>
public sealed partial class RoundupArchive
{
    static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseYamlFrontMatter()
        .DisableHtml()
        .Build();

    /// <summary>Reads every <c>NNN.md</c> file in <paramref name="folder"/>; a missing folder means no issues yet.</summary>
    public RoundupArchive(string folder, ILogger logger)
    {
        var issues = new List<RoundupIssue>();
        if (Directory.Exists(folder))
        {
            foreach (var path in Directory.EnumerateFiles(folder, "*.md").Where(p => IssueFileName().IsMatch(Path.GetFileName(p))))
            {
                try
                {
                    issues.Add(Parse(File.ReadAllText(path)));
                }
                catch (FormatException ex)
                {
                    logger.LogError("Skipped {File}: {Reason}", Path.GetFileName(path), ex.Message);
                }
            }
        }

        foreach (var duplicate in issues.GroupBy(i => i.Number).Where(g => g.Count() > 1))
            logger.LogError("More than one file says it is issue #{Number}; only the first is shown.", duplicate.Key);

        All = issues.GroupBy(i => i.Number).Select(g => g.First()).OrderByDescending(i => i.Number).ToList();
    }

    /// <summary>Every issue, newest first.</summary>
    public IReadOnlyList<RoundupIssue> All { get; }

    /// <summary>The newest issue, or null before the first one is published.</summary>
    public RoundupIssue? Latest => All.Count > 0 ? All[0] : null;

    /// <summary>The issue with this number, or null.</summary>
    public RoundupIssue? Find(int number) => All.FirstOrDefault(i => i.Number == number);

    /// <summary>The issue sent before this one, or null for the first.</summary>
    public RoundupIssue? Previous(RoundupIssue issue) => All.FirstOrDefault(i => i.Number < issue.Number);

    /// <summary>The issue sent after this one, or null for the latest.</summary>
    public RoundupIssue? Next(RoundupIssue issue) => All.LastOrDefault(i => i.Number > issue.Number);

    /// <summary>
    /// When the next issue is due: two weeks after the latest, as long as that's today or later.
    /// Null before the first issue, or when a send is overdue (so the site never shows a past date).
    /// </summary>
    public DateOnly? NextSend
    {
        get
        {
            if (Latest is null) return null;
            var next = Latest.Sent.AddDays(14);
            var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "America/New_York"));
            return next >= today ? next : null;
        }
    }

    /// <summary>Reads one issue file: front matter, then the Markdown body.</summary>
    public static RoundupIssue Parse(string text)
    {
        text = text.Replace("\r\n", "\n").TrimStart('﻿');
        if (!text.StartsWith("---\n", StringComparison.Ordinal))
            throw new FormatException("the file must start with front matter between --- lines.");

        var end = text.IndexOf("\n---", 3, StringComparison.Ordinal);
        if (end < 0) throw new FormatException("the front matter has no closing --- line.");

        var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var lists = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        string? listKey = null;
        foreach (var line in text[4..end].Split('\n'))
        {
            var trimmed = line.Trim();
            if (listKey is not null && trimmed.StartsWith("- ", StringComparison.Ordinal))
            {
                lists[listKey].Add(Unquote(trimmed[2..].Trim()));
                continue;
            }
            listKey = null;

            var colon = line.IndexOf(':');
            if (colon <= 0) continue;
            var key = line[..colon].Trim();
            var value = line[(colon + 1)..].Trim();
            if (value.Length == 0)
            {
                listKey = key;
                lists[key] = [];
            }
            else if (value.StartsWith('[') && value.EndsWith(']'))
            {
                lists[key] = value[1..^1].Split(',').Select(v => Unquote(v.Trim())).Where(v => v.Length > 0).ToList();
            }
            else
            {
                fields[key] = Unquote(value);
            }
        }

        IReadOnlyList<string> List(string name) => lists.TryGetValue(name, out var items) ? items : [];

        string Field(string name) =>
            fields.TryGetValue(name, out var value) && value.Length > 0
                ? value
                : throw new FormatException($"the front matter needs a \"{name}\" line.");

        if (!int.TryParse(Field("number"), NumberStyles.None, CultureInfo.InvariantCulture, out var number) || number < 1)
            throw new FormatException("\"number\" must be a whole number from 1 up.");
        if (!DateOnly.TryParseExact(Field("sent"), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var sent))
            throw new FormatException("\"sent\" must be a date written as yyyy-MM-dd.");

        var document = Markdown.Parse(text, Pipeline);
        if (document.FirstOrDefault(b => b is not Markdig.Extensions.Yaml.YamlFrontMatterBlock) is HeadingBlock { Level: 1 } title)
            document.Remove(title);

        return new RoundupIssue(number, sent, Field("subject"), Field("preview"), document.ToHtml(Pipeline),
            List("highlights").Take(3).ToList(), List("tags"));
    }

    static string Unquote(string value) =>
        value.Length >= 2 && value[0] == '"' && value[^1] == '"'
            ? value[1..^1].Replace("\\\"", "\"").Replace("\\\\", "\\")
            : value;

    [GeneratedRegex(@"^\d{3,}\.md$")]
    private static partial Regex IssueFileName();
}
