namespace BudgetCouple.Infrastructure.Services.Notifications;

using BudgetCouple.Application.Notifications.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class WebPushSender : IWebPushSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<WebPushSender> _logger;

    public WebPushSender(IConfiguration configuration, ILogger<WebPushSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(string titulo, string corpo, string? link, CancellationToken cancellationToken = default)
    {
        var publicKey = _configuration["WebPush:PublicKey"];
        var privateKey = _configuration["WebPush:PrivateKey"];

        if (string.IsNullOrEmpty(publicKey) || string.IsNullOrEmpty(privateKey))
        {
            _logger.LogWarning("WebPush keys not configured. Push notifications will not be sent (STUB MODE)");
            return;
        }

        try
        {
            _logger.LogInformation($"WebPush notification would be sent: {titulo}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending web push notification");
            throw;
        }

        await Task.CompletedTask;
    }

    public async Task<bool> IsConfiguredAsync()
    {
        var publicKey = _configuration["WebPush:PublicKey"];
        var privateKey = _configuration["WebPush:PrivateKey"];
        return !string.IsNullOrEmpty(publicKey) && !string.IsNullOrEmpty(privateKey);
    }
}
