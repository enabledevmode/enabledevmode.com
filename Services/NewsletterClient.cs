using System.Net.Http.Json;

namespace EnableDevMode.Services;

/// <summary>
/// Subscribes people to The Blazor Roundup through Kit's v4 API (https://developers.kit.com).
/// Registered in Program.cs with Kit's base address. Configuration:
/// <list type="bullet">
///   <item>Newsletter:ApiKey: a Kit v4 API key (user-secrets locally, a Fly secret in production).</item>
///   <item>Newsletter:FormId: the id of the Kit form, which has double opt-in turned on.</item>
/// </list>
/// </summary>
public sealed class NewsletterClient(HttpClient http, IConfiguration config, ILogger<NewsletterClient> log)
{
    /// <summary>
    /// Creates the subscriber as inactive, then adds them to the double opt-in form, which makes Kit
    /// send the confirmation email. They become active when they confirm.
    /// </summary>
    /// <param name="email">The address from the form.</param>
    /// <param name="source">Where they signed up (home, about, project:slug), for the logs.</param>
    /// <param name="referrer">The page URL; Kit stores it on the subscriber.</param>
    /// <returns>True when Kit accepted the signup.</returns>
    public async Task<bool> SubscribeAsync(string email, string source, string? referrer, CancellationToken ct = default)
    {
        // Missing settings are a deployment mistake, so fail loudly rather than pretend it worked.
        var apiKey = config["Newsletter:ApiKey"]
            ?? throw new InvalidOperationException("Newsletter:ApiKey is not configured.");
        var formId = config.GetValue<int>("Newsletter:FormId") is > 0 and var id
            ? id
            : throw new InvalidOperationException("Newsletter:FormId is not configured.");

        try
        {
            // 1. Create the subscriber. An address Kit already has keeps its current state.
            using (var created = await SendAsync(HttpMethod.Post, "v4/subscribers", apiKey,
                       new { email_address = email, state = "inactive" }, ct))
            {
                if (!created.IsSuccessStatusCode)
                    return Failed("create subscriber", created, source);
            }

            // 2. Add them to the form: Kit sends the confirmation (incentive) email.
            using (var added = await SendAsync(HttpMethod.Post, $"v4/forms/{formId}/subscribers", apiKey,
                       new { email_address = email, referrer }, ct))
            {
                if (!added.IsSuccessStatusCode)
                    return Failed("add to form", added, source);
            }

            return true;
        }
        catch (HttpRequestException ex)
        {
            log.LogError(ex, "Kit unreachable (signup from {Source})", source);
            return false;
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            // HttpClient's own timeout, not the visitor leaving.
            log.LogError(ex, "Kit timed out (signup from {Source})", source);
            return false;
        }
    }

    Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, string apiKey, object body, CancellationToken ct)
    {
        var request = new HttpRequestMessage(method, path) { Content = JsonContent.Create(body) };
        request.Headers.Add("X-Kit-Api-Key", apiKey);
        return http.SendAsync(request, ct);
    }

    bool Failed(string step, HttpResponseMessage response, string source)
    {
        // Never log the email address itself.
        log.LogWarning("Kit {Step} failed with status {Status} (signup from {Source})",
            step, (int)response.StatusCode, source);
        return false;
    }
}
