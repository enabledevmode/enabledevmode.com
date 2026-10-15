# enabledevmode.com

Source for [enabledevmode.com](https://enabledevmode.com), the website for the [Enable Dev Mode](https://www.youtube.com/@enabledevmode) YouTube channel. **Learn. Build. Ship.**

The site lists the channel's projects. Each project page summarizes what you'll build, links every lesson to its point in the video and to the code at that step, and collects tips learned along the way. It's also where you sign up for **The Blazor Roundup**, a short email every two weeks.

## Stack

- Blazor Web App on .NET 10, rendered with static server-side rendering (no WebAssembly, no SignalR connection)
- [IzzyBlazor](https://github.com/enabledevmode/izzyblazor) for every component, layout and theme. The site doubles as a working example of the library.
- Fonts: Sora and JetBrains Mono

## Run it locally

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/enabledevmode/enabledevmode.com.git
cd enabledevmode.com
dotnet watch
```

The site runs without any setup. Newsletter signups go to [Kit](https://kit.com) and need an API key, which is kept out of source control, and the form's id in `appsettings.json` (`Newsletter:FormId`):

```bash
dotnet user-secrets set "Newsletter:ApiKey" "<Kit v4 API key>"
```

## Project layout

```
Components/
  Layout/     MainLayout (IzzyBlazor's AppShell: header, main, footer)
  Pages/      Home, Projects, ProjectPage, About, NotFound
  Shared/     Wordmark, Brand, Logo, ProjectCard, LessonLog, StatusBadge, SignupForm, NewsletterBand
Data/         Project records, ProjectCatalog, SiteLinks
Services/     NewsletterClient (calls the email provider), Sitemap
wwwroot/      app.css (one override), images
```

## Adding a project

Projects are data, not pages. To add one:

1. Add a `Project` record to `Data/ProjectCatalog.cs`. The slug is lowercase with hyphens and matches the project's repo name, for example `signup-wizard` for `github.com/enabledevmode/signup-wizard`.
2. Add its cover and gallery images under `wwwroot/images/projects/<slug>/`.

The home page, the Projects page, the project page at `/projects/<slug>`, and the sitemap all pick it up automatically.

## Design system

The theme is one `IzzyTheme` tag in `Components/App.razor`: the Standard preset, the brand blue (`Brand="#005AB6"`), the page gray (`Gray="#F9F9FF"`, from which IzzyBlazor derives the light and dark gray ladders), pill-shaped controls (`Radius="ThemeRadius.Full"`), Sora and JetBrains Mono. `wwwroot/app.css` adds the brand's own dark-mode blues and the logo's colors. Layout and spacing come from component props (`Stack`, `Gap`, `Section`), not CSS.

## Newsletter

Signups use Kit's v4 API (`Services/NewsletterClient.cs`): each one creates the subscriber as inactive, then adds them to the Kit form, which has double opt-in turned on, so Kit sends the confirmation email. The page they signed up on is stored as the subscriber's referrer. Form posts are limited to 5 a minute per visitor.

## Deployment

The site runs on [Fly.io](https://fly.io) from the `Dockerfile`, with the settings in `fly.toml` (one always-on 512 MB machine). Every push to `main` deploys through `.github/workflows/fly-deploy.yml`.

First time:

```bash
fly launch --no-deploy                       # use the existing fly.toml
fly secrets set Newsletter__ApiKey=<key>     # the Kit API key
fly deploy
fly certs add enabledevmode.com              # then add the DNS records it prints
fly certs add www.enabledevmode.com
fly tokens create deploy                     # save as the FLY_API_TOKEN repository secret
```

## Contributing

Found a typo or a broken link? Issues and pull requests are welcome. Code for each project lives in its own repo in the [enabledevmode](https://github.com/enabledevmode) organization.
