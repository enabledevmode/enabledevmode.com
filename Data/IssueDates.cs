using System.Globalization;

namespace EnableDevMode.Data;

/// <summary>How issue dates read on the site, in one place.</summary>
public static class IssueDates
{
    /// <summary>"Oct 22, 2026": cards and lists.</summary>
    public static string Short(DateOnly day) => day.ToString("MMM d, yyyy", CultureInfo.InvariantCulture);

    /// <summary>"October 22, 2026": the issue page.</summary>
    public static string Long(DateOnly day) => day.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture);
}
