namespace BudgetCouple.Application.Notifications.Dtos;

using BudgetCouple.Domain.Notifications;

public class NotificationHistoryDto
{
    public Guid Id { get; set; }
    public NotificationChannel Canal { get; set; }
    public NotificationType Tipo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Corpo { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; }
    public string? Erro { get; set; }
    public DateTime EnviadoEm { get; set; }
}
