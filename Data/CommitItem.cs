namespace EnableDevMode.Data;

/// <summary>One row in a CommitLog.</summary>
public sealed record CommitItem(
    string Title,
    string Meta,
    string? Sha = null,
    string? CommitUrl = null,
    bool Latest = false,
    string? Description = null,
    string? WatchUrl = null,
    string? WatchLabel = null)
{
    /// <summary>Rows for a project page's lesson list.</summary>
    public static IReadOnlyList<CommitItem> FromLessons(Project project) =>
        project.Lessons.Select((lesson, i) => new CommitItem(
                Title: lesson.Title,
                Meta: $"Lesson {i + 1} · {(int)lesson.Duration.TotalMinutes} min",
                Sha: lesson.CommitSha,
                CommitUrl: project.CommitUrl(lesson),
                Description: lesson.Summary,
                WatchUrl: project.WatchUrl(lesson),
                WatchLabel: lesson.Start.ToString(lesson.Start.TotalHours >= 1 ? @"h\:mm\:ss" : @"mm\:ss")))
            .ToList();
}