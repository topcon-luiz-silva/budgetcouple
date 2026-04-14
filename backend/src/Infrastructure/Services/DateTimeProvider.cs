namespace BudgetCouple.Infrastructure.Services;

using BudgetCouple.Application.Common.Interfaces;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
