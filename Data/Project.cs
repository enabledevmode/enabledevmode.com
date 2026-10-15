using System.Web;

namespace EnableDevMode.Data;

public enum ProjectStatus { Planned, Coming, Published }

public sealed record Lesson(
    string Title,
    string Summary,
    TimeSpan Start,          // where it starts in the project video
    TimeSpan Duration,
    string? CommitSha,       // short hash of the commit at the end of this lesson
    DateOnly? Published = null);

/// <summary>A tip or gotcha. Code is an optional short snippet shown under it.</summary>
public sealed record Tip(string Title, string Body, string? Code = null);

public sealed record Resource(string Title, string Url);

public sealed record Project(
    string Slug,             // lowercase-with-hyphens; matches the repo name
    string Title,
    string Summary,
    ProjectStatus Status,
    DateOnly? ReleaseDate,
    string DotNetVersion,
    string VideoUrl,         // https://www.youtube.com/watch?v=... (empty until the video is out)
    string RepoUrl,          // e.g. https://github.com/enabledevmode/signup-wizard
    string? DemoUrl,
    string Cover,            // e.g. images/projects/signup-wizard/cover.png
    string Description,
    IReadOnlyList<string> Gallery,
    IReadOnlyList<string> Learn,
    IReadOnlyList<string> Prerequisites,
    IReadOnlyList<Lesson> Lessons,
    IReadOnlyList<Tip> Tips,
    IReadOnlyList<Resource> Resources)
{
    public string? CommitUrl(Lesson lesson) =>
        lesson.CommitSha is null ? null : $"{RepoUrl}/commit/{lesson.CommitSha}";

    public string WatchUrl(Lesson lesson) =>
        $"{VideoUrl}&t={(int)lesson.Start.TotalSeconds}s";

    /// <summary>The YouTube video ID from VideoUrl, for the embedded player.</summary>
    public string? VideoId =>
        Uri.TryCreate(VideoUrl, UriKind.Absolute, out var uri)
            ? uri.Host == "youtu.be" ? uri.AbsolutePath.Trim('/') : HttpUtility.ParseQueryString(uri.Query)["v"]
            : null;

    public TimeSpan TotalDuration => Lessons.Aggregate(TimeSpan.Zero, (sum, l) => sum + l.Duration);
}
