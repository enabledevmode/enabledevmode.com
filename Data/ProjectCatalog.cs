namespace EnableDevMode.Data;

public static class ProjectCatalog
{
    public static IReadOnlyList<Project> All { get; } =
    [
        // new Project(Slug: "signup-wizard", Title: "Multi-step signup wizard with EditForm", ...),
    ];

    public static Project? Find(string slug) => All.FirstOrDefault(p => p.Slug == slug);

    /// <summary>Most recently published lessons across all projects, newest first.</summary>
    public static IReadOnlyList<CommitItem> LatestLessons(int count) =>
        All.SelectMany(p => p.Lessons.Select((l, i) => (Project: p, Lesson: l, Number: i + 1)))
            .Where(x => x.Lesson.Published is not null)
            .OrderByDescending(x => x.Lesson.Published)
            .Take(count)
            .Select((x, i) => new CommitItem(
                Title: x.Lesson.Title,
                Meta: $"{x.Project.Title} · Lesson {x.Number}",
                Sha: x.Lesson.CommitSha,
                CommitUrl: x.Project.CommitUrl(x.Lesson),
                Latest: i == 0))
            .ToList();
}