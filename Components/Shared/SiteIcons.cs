using IzzyBlazor;

namespace EnableDevMode.Components.Shared;

/// <summary>
/// Outline YouTube and GitHub icons, as drawn in the site mockups: a 24px grid stroked in
/// currentColor, to sit with the Lucide icons used elsewhere. Lucide no longer ships brand icons and
/// IzzyBlazor's Brands are solid, so the site keeps these two. The GitHub mark is Feather's (MIT).
/// Both logos are their owners' trademarks; use them only to link to those services.
/// </summary>
public static class SiteIcons
{
    /// <summary>A rounded screen with a play triangle.</summary>
    public static IconData YouTube { get; } = new("0 0 24 24",
        "<rect x=\"2.5\" y=\"5.5\" width=\"19\" height=\"13\" rx=\"4\"/><path d=\"M10 9.5v5l4.5-2.5z\"/>",
        IconStyle.Stroke, StrokeWidth: 1.8, Name: "youtube");

    /// <summary>The Octocat, outlined.</summary>
    public static IconData GitHub { get; } = new("0 0 24 24",
        "<path d=\"M9 19c-5 1.5-5-2.5-7-3m14 6v-3.87a3.37 3.37 0 0 0-.94-2.61c3.14-.35 6.44-1.54 6.44-7A5.44 5.44 0 0 0 20 4.77 5.07 5.07 0 0 0 19.91 1S18.73.65 16 2.48a13.38 13.38 0 0 0-7 0C6.27.65 5.09 1 5.09 1A5.07 5.07 0 0 0 5 4.77a5.44 5.44 0 0 0-1.5 3.78c0 5.42 3.3 6.61 6.44 7A3.37 3.37 0 0 0 9 18.13V22\"/>",
        IconStyle.Stroke, StrokeWidth: 1.8, Name: "github");
}
