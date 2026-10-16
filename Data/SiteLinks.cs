namespace EnableDevMode.Data;

/// <summary>
/// External links used across the site (header, footer, About page, project pages).
/// Change them here and every page picks up the new value.
/// </summary>
public static class SiteLinks
{
    public const string YouTube = "https://www.youtube.com/@enabledevmode";
    public const string GitHub = "https://github.com/enabledevmode";
    public const string IzzyBlazor = "https://github.com/enabledevmode/izzyblazor";
    public const string Email = "israel@enabledevmode.com";
    public const string TtkBootstrap = "https://www.ttkbootstrap.org";
    public const string Bootstack = "https://www.bootstack.org";

    /// <summary>The email address as a mailto: link.</summary>
    public const string EmailHref = "mailto:" + Email;
}
