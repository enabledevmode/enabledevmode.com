# enabledevmode.com

Source for [enabledevmode.com](https://enabledevmode.com), the website for the [Enable Dev Mode](https://www.youtube.com/@enabledevmode) YouTube channel. **Learn. Build. Ship.**

The site lists the channel's projects. Each project page summarizes what you'll build, links every lesson to its point in the video and to the code at that step, and collects tips learned along the way. It's also where you sign up for **The Blazor Roundup**, a short email every two weeks.

## Stack

- Blazor Web App on .NET 10, rendered with static server-side rendering (no WebAssembly, no SignalR connection)
- A handful of plain Razor components and one stylesheet, `wwwroot/site.css`. No component library.
- Fonts: Sora and JetBrains Mono

## Run it locally

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/enabledevmode/enabledevmode.com.git
cd enabledevmode.com
dotnet watch
```

The site runs without any setup. Newsletter signups need an API key for the email provider, which is kept out of source control:

```bash
dotnet user-secrets set "Newsletter:ApiKey" "<key>"
```

## Project layout

```
Components/
  Layout/     MainLayout, SiteHeader, SiteFooter
  Pages/      Home, Projects, ProjectPage, About, NotFound
  Shared/     Button, Badge, CommitLog, ProjectCard, SignupForm, ...
Data/         Project records, ProjectCatalog, SiteLinks
Services/     NewsletterClient (calls the email provider)
wwwroot/      site.css, images
```

## Adding a project

Projects are data, not pages. To add one:

1. Add a `Project` record to `Data/ProjectCatalog.cs`. The slug is lowercase with hyphens and matches the project's repo name, for example `signup-wizard` for `github.com/enabledevmode/signup-wizard`.
2. Add its cover and gallery images under `wwwroot/images/projects/<slug>/`.

The home page, the Projects page, the project page at `/projects/<slug>`, and the sitemap all pick it up automatically.

## Design system

Colors, type, spacing, and radius are CSS custom properties at the top of `wwwroot/site.css`, with light and dark values. Components use those tokens and the classes in the same file; avoid hard-coded colors or spacing.

## Deployment

Any host that runs ASP.NET Core works. Set the newsletter key as an environment variable:

```
Newsletter__ApiKey=<key>
```

## Contributing

Found a typo or a broken link? Issues and pull requests are welcome. Code for each project lives in its own repo in the [enabledevmode](https://github.com/enabledevmode) organization.
