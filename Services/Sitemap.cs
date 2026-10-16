using System.Xml.Linq;
using EnableDevMode.Data;

namespace EnableDevMode.Services;

/// <summary>
/// Builds /sitemap.xml from the fixed pages, the Roundup issues and, once one is published, the
/// projects.
/// </summary>
public static class Sitemap
{
    static readonly XNamespace Ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

    public static IResult Build(HttpContext http, IEnumerable<RoundupIssue> issues, IEnumerable<Project> projects)
    {
        var root = $"{http.Request.Scheme}://{http.Request.Host}";
        List<string> paths = ["/", "/roundup", .. issues.Select(i => i.Url), "/about"];
        if (ProjectCatalog.HasPublished)
            paths.AddRange(["/projects", .. projects.Select(p => $"/projects/{p.Slug}")]);

        var xml = new XDocument(new XElement(Ns + "urlset",
            paths.Select(path => new XElement(Ns + "url", new XElement(Ns + "loc", root + path)))));

        return Results.Text(xml.Declaration + xml.ToString(), "application/xml");
    }
}
