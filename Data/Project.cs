namespace EnableDevMode.Data;

public enum ProjectStatus
{
    Planned,
    Coming,
    Published
}

public sealed record Lesson(
    string Title,
    string Summary,
    TimeSpan Start, // where it starts in the project video
    TimeSpan Duration,
    string? CommitSha, // short hash of the commit at the end of this lesson
    DateOnly? Published = null);

public sealed record Tip(string Title, string Body);

public sealed record Resource(string Title, string Url);

public sealed record Project(
    string Slug,
    string Title,
    string Summary,
    ProjectStatus Status,
    DateOnly? ReleaseDate,
    string DotNetVersion,
    string VideoUrl, // e.g. https://www.youtube.com/watch?v=...
    string RepoUrl, // e.g. https://github.com/enabledevmode/signup-wizard
    string? DemoUrl,
    string Cover,
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
}