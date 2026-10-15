namespace EnableDevMode.Data;

/// <summary>
/// External links used across the site (header, footer, About page, project pages).
/// Change them here and every page picks up the new value.
/// </summary>
public static class SiteLinks
{
    public const string YouTube = "https://www.youtube.com/@enabledevmode";
    public const string GitHub = "https://github.com/enabledevmode";
    public const string Email = "israel@enabledevmode.com";

    /// <summary>The email address as a mailto: link.</summary>
    public const string EmailHref = "mailto:" + Email;
}
