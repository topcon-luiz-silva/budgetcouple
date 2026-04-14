namespace BudgetCouple.Infrastructure.Services.Notifications;

using BudgetCouple.Application.Notifications.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class ResendEmailSender : IEmailSender
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ResendEmailSender> _logger;
    private readonly string? _apiKey;

    public ResendEmailSender(HttpClient httpClient, IConfiguration configuration, ILogger<ResendEmailSender> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _apiKey = configuration["Resend:ApiKey"];
    }

    public async Task SendAsync(string emailAddress, string titulo, string corpo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogWarning("Resend API Key not configured. Email will not be sent (STUB MODE)");
            return;
        }

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");

            var payload = new
            {
                from = "noreply@budgetcouple.local",
                to = emailAddress,
                subject = titulo,
                html = $"<p>{corpo}</p>"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError($"Resend API error: {response.StatusCode} - {errorContent}");
                throw new Exception($"Failed to send email: {response.StatusCode}");
            }

            _logger.LogInformation($"Email sent successfully to {emailAddress}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending email to {emailAddress}");
            throw;
        }
    }

    public async Task<bool> IsConfiguredAsync()
    {
        return !string.IsNullOrEmpty(_apiKey);
    }
}
