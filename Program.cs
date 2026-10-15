using EnableDevMode.Components;
using EnableDevMode.Data;
using EnableDevMode.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();
builder.Services.AddIzzyBlazor();
builder.Services.AddHttpClient<NewsletterClient>();
builder.Services.AddOutputCache();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();
app.UseOutputCache();   // only the sitemap opts in; pages carry a form, so they aren't cached

app.MapStaticAssets();
app.MapGet("/sitemap.xml", (HttpContext http) => Sitemap.Build(http, ProjectCatalog.All))
   .CacheOutput(p => p.Expire(TimeSpan.FromHours(1)));
app.MapRazorComponents<App>();
app.MapIzzyBlazor();

app.Run();
