namespace BudgetCouple.Domain.Notifications;

using BudgetCouple.Domain.Common;

public enum NotificationStatus
{
    Success,
    Failed,
    Pending
}

public enum NotificationChannel
{
    Email,
    WebPush,
    Telegram
}

public enum NotificationType
{
    VencimentoProximo,
    AlertaOrcamento,
    FaturaProxima
}

public class NotificationHistory : Entity
{
    public NotificationChannel Canal { get; private set; }
    public NotificationType Tipo { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string Corpo { get; private set; } = string.Empty;
    public NotificationStatus Status { get; private set; }
    public string? Erro { get; private set; }
    public DateTime EnviadoEm { get; private set; }

    // Constructor
    private NotificationHistory() { }

    public static NotificationHistory Create(
        NotificationChannel canal,
        NotificationType tipo,
        string titulo,
        string corpo,
        NotificationStatus status = NotificationStatus.Pending,
        string? erro = null)
    {
        return new NotificationHistory
        {
            Canal = canal,
            Tipo = tipo,
            Titulo = titulo,
            Corpo = corpo,
            Status = status,
            Erro = erro,
            EnviadoEm = DateTime.UtcNow
        };
    }

    public void MarcarComSucesso()
    {
        Status = NotificationStatus.Success;
        Erro = null;
    }

    public void MarcarComErro(string mensagemErro)
    {
        Status = NotificationStatus.Failed;
        Erro = mensagemErro;
    }
}
