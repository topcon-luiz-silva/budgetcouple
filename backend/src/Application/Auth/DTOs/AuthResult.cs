namespace BudgetCouple.Application.Auth.DTOs;

/// <summary>
/// Result of successful authentication containing JWT token and expiration.
/// </summary>
public record AuthResult(string Token, DateTime ExpiresAt);
