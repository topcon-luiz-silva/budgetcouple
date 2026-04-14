namespace BudgetCouple.Application.Auth.DTOs;

/// <summary>
/// Current authentication status including PIN configuration and lock state.
/// </summary>
public record AuthStatusDto(bool PinConfigured, bool Locked, DateTime? LockedUntil);
