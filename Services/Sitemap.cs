using System.Xml.Linq;
using EnableDevMode.Data;

namespace EnableDevMode.Services;

/// <summary>Builds /sitemap.xml from the fixed pages and the project catalog.</summary>
public static class Sitemap
{
    static readonly XNamespace Ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

    public static IResult Build(HttpContext http, IEnumerable<Project> projects)
    {
        var root = $"{http.Request.Scheme}://{http.Request.Host}";
        string[] paths = ["/", "/projects", "/about", .. projects.Select(p => $"/projects/{p.Slug}")];

        var xml = new XDocument(new XElement(Ns + "urlset",
            paths.Select(path => new XElement(Ns + "url", new XElement(Ns + "loc", root + path)))));

        return Results.Text(xml.Declaration + xml.ToString(), "application/xml");
    }
}
