namespace EnableDevMode.Services;

public sealed class NewsletterClient(HttpClient http, IConfiguration config, ILogger<NewsletterClient> log)
{
    public async Task SubscribeAsync(string email, string source, CancellationToken ct = default)
    {
        var apiKey = config["Newsletter:ApiKey"] 
                     ?? throw new InvalidOperationException("Newsletter:ApiKey is not configured.");
        
        // Provider-specific: set the endpoint, auth header, and body for kit or Buttondown here,
        // and pass `source` as a tag so you know where people signed up.
        _ = (http, apiKey);
        log.LogInformation("Newsletter signup from {Source}", source);
        await Task.CompletedTask;
    }
}
