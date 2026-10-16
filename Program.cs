using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using EnableDevMode.Components;
using EnableDevMode.Data;
using EnableDevMode.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();
builder.Services.AddIzzyBlazor();
builder.Services.AddHttpClient<NewsletterClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Newsletter:BaseUrl"] ?? "https://api.kit.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

// Form posts (the newsletter signup) are limited to 5 a minute per visitor, to slow down bots.
// Fly's proxy passes the visitor's address in Fly-Client-IP.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = (context, ct) => new ValueTask(context.HttpContext.Response.WriteAsync(
        "Too many signups from your address. Please try again in a minute.", ct));
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(http =>
        HttpMethods.IsPost(http.Request.Method)
            ? RateLimitPartition.GetFixedWindowLimiter(
                http.Request.Headers["Fly-Client-IP"].FirstOrDefault()
                    ?? http.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1) })
            : RateLimitPartition.GetNoLimiter("get"));
});
builder.Services.AddOutputCache();

// The Blazor Roundup's issues, read once from Content/roundup at startup. A new issue goes live
// with the deploy that adds its file.
builder.Services.AddSingleton(sp => new RoundupArchive(
    Path.Combine(sp.GetRequiredService<IWebHostEnvironment>().ContentRootPath, "Content", "roundup"),
    sp.GetRequiredService<ILogger<RoundupArchive>>()));

// Fly's proxy passes the original scheme and client address; trust them so the sitemap and
// redirects use https.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
// Fly's proxy terminates HTTPS (force_https in fly.toml), so the app itself serves plain HTTP.
if (app.Environment.IsDevelopment()) app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAntiforgery();
app.UseOutputCache();   // only the sitemap and feed opt in; pages carry a form, so they aren't cached

app.MapStaticAssets();
app.MapGet("/sitemap.xml", (HttpContext http, RoundupArchive roundup) => Sitemap.Build(http, roundup.All, ProjectCatalog.All))
   .CacheOutput(p => p.Expire(TimeSpan.FromHours(1)));
app.MapGet("/roundup/feed.xml", (HttpContext http, RoundupArchive roundup) => RoundupFeed.Build(http, roundup.All))
   .CacheOutput(p => p.Expire(TimeSpan.FromHours(1)));
app.MapRazorComponents<App>();
app.MapIzzyBlazor();

app.Run();
