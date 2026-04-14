using MediatR;

namespace BudgetCouple.Domain.Common;

public interface IDomainEvent : INotification
{
    DateTime OcorridoEm { get; }
}

public abstract record DomainEvent : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}
