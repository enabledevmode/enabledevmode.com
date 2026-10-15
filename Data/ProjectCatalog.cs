namespace EnableDevMode.Data;

public static class ProjectCatalog
{
    public static IReadOnlyList<Project> All { get; } =
    [
        // Example (copy, uncomment, and fill in):
        /*
        new Project(
            Slug: "signup-wizard",
            Title: "Multi-step signup wizard with EditForm",
            Summary: "Validation, async checks, and conditional steps with EditForm and data annotations, from the first commit to deployment.",
            Status: ProjectStatus.Coming,
            ReleaseDate: new DateOnly(2026, 11, 3),
            DotNetVersion: "10",
            VideoUrl: "",
            RepoUrl: "https://github.com/enabledevmode/signup-wizard",
            DemoUrl: "https://example.com",
            Cover: "images/placeholder.svg",
            Description: "A signup flow that collects account details over several steps, checks the email against the server as you type, and skips steps that don't apply.",
            Gallery: ["images/placeholder.svg", "images/placeholder.svg", "images/placeholder.svg"],
            Learn: ["Multi-step forms with EditForm", "Validation with data annotations", "Async checks against the server", "Deploying a Blazor Web App"],
            Prerequisites: [".NET 10 SDK", "Visual Studio, Rider, or VS Code", "Basic C# and Razor"],
            Lessons:
            [
                new("Project setup", "Create the Blazor Web App, set up the folders, and plan the wizard's steps.", TimeSpan.Zero, TimeSpan.FromMinutes(8), "0a1b2c3", new DateOnly(2026, 10, 1)),
                new("The wizard shell", "Build the step container, the progress indicator, and Next and Back navigation.", TimeSpan.FromSeconds(492), TimeSpan.FromMinutes(14), "c7d8e9f", new DateOnly(2026, 10, 5)),
                new("Form model and validation", "Model each step with data annotations and show errors as people type.", TimeSpan.FromSeconds(1360), TimeSpan.FromMinutes(18), "e4f5a6b", new DateOnly(2026, 10, 8)),
                new("Deploy", "Publish the finished wizard and walk through what changes in production.", TimeSpan.FromSeconds(3885), TimeSpan.FromMinutes(32), "b3a4c5d"),
            ],
            Tips:
            [
                new("Give each form its own FormName", "Two forms on one static page with the same name can't be told apart when submitted.",
                    "<SignupForm FormName=\"signup-hero\" />\n<SignupForm FormName=\"signup-footer\" />"),
            ],
            Resources: [new("Blazor forms and validation docs", "https://learn.microsoft.com/aspnet/core/blazor/forms/")])
        */
    ];

    /// <summary>Finds a project by slug, ignoring case, so /projects/Signup-Wizard still works.</summary>
    public static Project? Find(string slug) =>
        All.FirstOrDefault(p => string.Equals(p.Slug, slug, StringComparison.OrdinalIgnoreCase));

    /// <summary>The project after this one in the catalog, or null for the last one.</summary>
    public static Project? Next(Project current)
    {
        for (var i = 0; i < All.Count - 1; i++)
            if (All[i].Slug == current.Slug) return All[i + 1];
        return null;
    }

    /// <summary>Most recently published lessons across all projects, newest first.</summary>
    public static IReadOnlyList<CommitItem> LatestLessons(int count) =>
        All.SelectMany(p => p.Lessons.Select((l, i) => (Project: p, Lesson: l, Number: i + 1)))
           .Where(x => x.Lesson.Published is not null)
           .OrderByDescending(x => x.Lesson.Published)
           .Take(count)
           .Select((x, i) => new CommitItem(
               Title: x.Lesson.Title,
               Meta: $"Lesson {x.Number} · {x.Project.Title}",
               Sha: x.Lesson.CommitSha,
               CommitUrl: x.Project.CommitUrl(x.Lesson),
               Latest: i == 0))
           .ToList();
}
