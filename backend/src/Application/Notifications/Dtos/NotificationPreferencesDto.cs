namespace BudgetCouple.Application.Notifications.Dtos;

public class NotificationPreferencesDto
{
    public bool EmailHabilitado { get; set; }
    public string? EmailEndereco { get; set; }
    public bool WebPushHabilitado { get; set; }
    public bool TelegramHabilitado { get; set; }
    public string? TelegramChatId { get; set; }
    public bool NotificarVencimentos1Dia { get; set; }
    public bool NotificarVencimentosDia { get; set; }
    public bool NotificarAlertasOrcamento { get; set; }
    public bool NotificarFaturas { get; set; }
}
