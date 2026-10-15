using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace EnableDevMode.Services;

/// <summary>
/// Talks to the email provider. Register with builder.Services.AddHttpClient&lt;NewsletterClient&gt;().
/// The API key comes from configuration ("Newsletter:ApiKey"), never from source control.
/// </summary>
public sealed class NewsletterClient(HttpClient http, IConfiguration config, ILogger<NewsletterClient> log)
{
    /// <summary>Adds the address to the list, tagged with where they signed up.</summary>
    /// <returns>True when the provider accepted the signup.</returns>
    public async Task<bool> SubscribeAsync(string email, string source, CancellationToken ct = default)
    {
        // A missing key is a deployment mistake, so fail loudly rather than pretend it worked.
        var apiKey = config["Newsletter:ApiKey"]
            ?? throw new InvalidOperationException("Newsletter:ApiKey is not configured.");

        // Provider-specific: replace the URL and body with your provider's subscribe call.
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example.com/subscribers")
        {
            Content = JsonContent.Create(new { email, tags = new[] { source } })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        try
        {
            using var response = await http.SendAsync(request, ct);
            if (response.IsSuccessStatusCode) return true;

            // Never log the email address itself.
            log.LogWarning("Newsletter signup from {Source} failed with status {Status}",
                source, (int)response.StatusCode);
            return false;
        }
        catch (HttpRequestException ex)
        {
            log.LogError(ex, "Newsletter provider unreachable (signup from {Source})", source);
            return false;
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            // HttpClient's own timeout, not the visitor leaving.
            log.LogError(ex, "Newsletter provider timed out (signup from {Source})", source);
            return false;
        }
    }
}
