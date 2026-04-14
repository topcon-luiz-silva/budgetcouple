namespace BudgetCouple.Infrastructure.Services.Notifications;

using BudgetCouple.Application.Notifications.Interfaces;
using BudgetCouple.Domain.Notifications;
using BudgetCouple.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class DailyNotificationJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DailyNotificationJob> _logger;

    public DailyNotificationJob(IServiceProvider serviceProvider, ILogger<DailyNotificationJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Execute()
    {
        _logger.LogInformation("Starting daily notification job");

        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                var preferencesRepository = scope.ServiceProvider.GetRequiredService<INotificationPreferencesRepository>();

                // Check preferences
                var preferences = await preferencesRepository.GetAsync();
                if (preferences == null)
                {
                    _logger.LogWarning("No notification preferences found");
                    return;
                }

                // Check for upcoming due dates
                if (preferences.NotificarVencimentos1Dia || preferences.NotificarVencimentosDia)
                {
                    await ProcessLancamentoVencimentos(notificationService, null, preferences);
                }

                // Check for budget alerts
                if (preferences.NotificarAlertasOrcamento)
                {
                    await ProcessBudgetAlerts(notificationService);
                }

                _logger.LogInformation("Daily notification job completed successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in daily notification job");
        }
    }

    private async Task ProcessLancamentoVencimentos(
        INotificationService notificationService,
        object lancamentoRepository,
        NotificationPreferences preferences)
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            // TODO: Get lancamentos from database
            // This is a placeholder until we integrate with the actual repository
            _logger.LogInformation("Processing lancamento vencimentos");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing lancamento vencimentos");
        }

        await Task.CompletedTask;
    }

    private async Task ProcessBudgetAlerts(INotificationService notificationService)
    {
        try
        {
            // This would integrate with the Fase 8 budget analysis
            // For now, it's a placeholder
            _logger.LogInformation("Budget alerts processing completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing budget alerts");
        }

        await Task.CompletedTask;
    }
}
