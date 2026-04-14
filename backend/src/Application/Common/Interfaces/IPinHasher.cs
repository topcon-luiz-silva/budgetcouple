namespace BudgetCouple.Application.Common.Interfaces;

/// <summary>
/// Interface for PIN hashing and verification.
/// </summary>
public interface IPinHasher
{
    string Hash(string pin);
    bool Verify(string pin, string hash);
}
