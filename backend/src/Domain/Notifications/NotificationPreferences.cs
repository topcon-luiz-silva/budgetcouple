namespace BudgetCouple.Domain.Notifications;

using BudgetCouple.Domain.Common;

public class NotificationPreferences : AggregateRoot
{
    public bool EmailHabilitado { get; private set; } = false;
    public string? EmailEndereco { get; private set; }

    public bool WebPushHabilitado { get; private set; } = false;

    public bool TelegramHabilitado { get; private set; } = false;
    public string? TelegramChatId { get; private set; }

    public bool NotificarVencimentos1Dia { get; private set; } = true;
    public bool NotificarVencimentosDia { get; private set; } = true;
    public bool NotificarAlertasOrcamento { get; private set; } = true;
    public bool NotificarFaturas { get; private set; } = true;

    // Constructor
    private NotificationPreferences() { }

    public static NotificationPreferences Create()
    {
        return new NotificationPreferences();
    }

    // Methods
    public void AtualizarEmailPreferences(bool habilitado, string? endereco)
    {
        EmailHabilitado = habilitado;
        EmailEndereco = endereco;
    }

    public void AtualizarWebPushPreferences(bool habilitado)
    {
        WebPushHabilitado = habilitado;
    }

    public void AtualizarTelegramPreferences(bool habilitado, string? chatId)
    {
        TelegramHabilitado = habilitado;
        TelegramChatId = chatId;
    }

    public void AtualizarTiposNotificacao(bool vencimentos1Dia, bool vencimentosDia, bool alertasOrcamento, bool faturas)
    {
        NotificarVencimentos1Dia = vencimentos1Dia;
        NotificarVencimentosDia = vencimentosDia;
        NotificarAlertasOrcamento = alertasOrcamento;
        NotificarFaturas = faturas;
    }
}
