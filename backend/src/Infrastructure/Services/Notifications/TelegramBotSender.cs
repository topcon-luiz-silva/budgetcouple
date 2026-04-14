namespace BudgetCouple.Infrastructure.Services.Notifications;

using BudgetCouple.Application.Notifications.Interfaces;
using BudgetCouple.Application.Notifications.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class TelegramBotSender : ITelegramSender
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TelegramBotSender> _logger;
    private readonly string? _botToken;

    public TelegramBotSender(HttpClient httpClient, IConfiguration configuration, ILogger<TelegramBotSender> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _botToken = configuration["Telegram:BotToken"];
    }

    public async Task SendAsync(string titulo, string corpo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_botToken))
        {
            _logger.LogWarning("Telegram BotToken not configured. Messages will not be sent (STUB MODE)");
            return;
        }

        try
        {
            // Get chat id from preferences (would need to pass as parameter in real usage)
            var message = $"<b>{titulo}</b>\n\n{corpo}";

            // In real implementation, we'd iterate through stored chat IDs
            // For now, this is stubbed
            _logger.LogInformation($"Telegram message would be sent: {titulo}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending Telegram message");
            throw;
        }

        await Task.CompletedTask;
    }

    public async Task<bool> IsConfiguredAsync()
    {
        return !string.IsNullOrEmpty(_botToken);
    }

    private async Task SendMessageAsync(string chatId, string message, CancellationToken cancellationToken)
    {
        try
        {
            var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";
            var payload = new
            {
                chat_id = chatId,
                text = message,
                parse_mode = "HTML"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError($"Telegram API error: {response.StatusCode} - {errorContent}");
                throw new Exception($"Failed to send Telegram message: {response.StatusCode}");
            }

            _logger.LogInformation($"Telegram message sent successfully to chat {chatId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending Telegram message to chat {chatId}");
            throw;
        }
    }
}
