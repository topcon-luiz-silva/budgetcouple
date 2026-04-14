namespace BudgetCouple.Application.Common.Interfaces;

/// <summary>
/// Interface for JWT token generation.
/// </summary>
public interface IJwtTokenService
{
    string GenerateToken();
}
